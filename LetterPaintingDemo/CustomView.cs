using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace LetterPaintingDemo
{
    public class CustomView : View
    {
        private Symbol _jieut;
        private List<Stroke> _jieutStrokes = new List<Stroke>();
        private List<Vector2> jieutStroke1Points = new List<Vector2>()
        {
            new Vector2(18, 25),
            new Vector2(100, 25),
        };
        private List<Vector2> jieutStroke2Points = new List<Vector2>()
        {
            new Vector2(63, 32),
            new Vector2(56, 68),
            new Vector2(40, 88),
            new Vector2(19, 98),
        };
        private List<Vector2> jieutStroke3Points = new List<Vector2>()
        {
            new Vector2(67, 60),
            new Vector2(88, 88),
            new Vector2(108, 98),
        };

        private Path _path = new Path();
        private Paint _paint = new Paint();
        private Paint _chkptPaint = new Paint();
        private Bitmap _maskBitmap;
        private Bitmap _greenStrokesBitmap;
        private Bitmap _redStrokesBitmap;
        private Canvas _greenStrokesCanvas;
        private Canvas _redStrokesCanvas;

        private const float CHKPT_RADIUS_SQR = 50 * 50;
        private int _buffer;
        private int _currentStokeIdx = 0;
        private int _currentChkPtIdx = 0;
        private bool _complete;
        private Stroke _currentStroke;
        private Vector2 _currentChkPt;
        private Vector2 _nextChkPt;
        private Vector2 _chkPtVec;

        public CustomView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            Init(w, h, oldw, oldh);
        }

        private void Init(int w, int h, int oldw, int oldh)
        {
            _maskBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.jieut);
            _maskBitmap = Bitmap.CreateScaledBitmap(_maskBitmap, w, w, true);
            _greenStrokesBitmap = Bitmap.CreateBitmap(_maskBitmap.Width, _maskBitmap.Height, Bitmap.Config.Argb8888);
            _greenStrokesCanvas = new Canvas(_greenStrokesBitmap);
            _redStrokesBitmap = Bitmap.CreateBitmap(_maskBitmap.Width, _maskBitmap.Height, Bitmap.Config.Argb8888);
            _redStrokesCanvas = new Canvas(_redStrokesBitmap);

            float scale = w / 128f;
            _paint.StrokeWidth = 15f * scale;
            _paint.Color = Color.LawnGreen;
            _paint.AntiAlias = true;
            _paint.SetStyle(Paint.Style.Stroke);
            _paint.StrokeJoin = Paint.Join.Round;
            _paint.StrokeCap = Paint.Cap.Round;

            _chkptPaint.StrokeWidth = 20;
            _chkptPaint.Color = Color.AliceBlue;
            _chkptPaint.AntiAlias = true;

            for (int i = 0; i < jieutStroke1Points.Count; ++i)
            {
                jieutStroke1Points[i] = jieutStroke1Points[i] * scale;
            }
            for (int i = 0; i < jieutStroke2Points.Count; ++i)
            {
                jieutStroke2Points[i] = jieutStroke2Points[i] * scale;
            }
            for (int i = 0; i < jieutStroke3Points.Count; ++i)
            {
                jieutStroke3Points[i] = jieutStroke3Points[i] * scale;
            }
            var stroke1 = new Stroke(jieutStroke1Points);
            var stroke2 = new Stroke(jieutStroke2Points);
            var stroke3 = new Stroke(jieutStroke3Points);
            _jieutStrokes.Add(stroke1);
            _jieutStrokes.Add(stroke2);
            _jieutStrokes.Add(stroke3);
            _jieut = new Symbol(_jieutStrokes);

            _currentStroke = _jieut.Strokes[_currentStokeIdx];
            _currentChkPt = _currentStroke.ChkPts[_currentChkPtIdx];
            _nextChkPt = _currentStroke.ChkPts[_currentChkPtIdx + 1];
            _chkPtVec = _nextChkPt - _currentChkPt;

            DrawChkptCircles();
        }

        protected override void OnDraw(Canvas canvas)
        {
            _paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstOver));
            _paint.Color = Color.Red;
            _redStrokesCanvas.DrawPath(_path, _paint);
            _paint.Color = Color.LawnGreen;
            _greenStrokesCanvas.DrawPath(_path, _paint);

            // Painted strokes above are the destinations.
            // Now, color those destinations in the shape of the sources.
            _paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));
            // _maskBitmap is the source
            _greenStrokesCanvas.DrawBitmap(_maskBitmap, 0, 0, _paint);
            _paint.SetXfermode(null);
            canvas.DrawBitmap(_maskBitmap, 0, 0, _paint);
            canvas.DrawBitmap(_redStrokesBitmap, 0, 0, _paint);
            canvas.DrawBitmap(_greenStrokesBitmap, 0, 0, _paint);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var touchX = e.GetX();
            var touchY = e.GetY();

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _path.MoveTo(touchX, touchY);
                    VerifyStartAtChkpt(new Vector2(touchX, touchY));
                    break;
                case MotionEventActions.Move:
                    _path.LineTo(touchX, touchY);
                    ++_buffer;
                    if (_buffer >= 2)
                    {
                        Calculate(new Vector2(touchX, touchY));
                        StatusCheck();
                    }
                    break;
                case MotionEventActions.Up:
                    break;
                default:
                    return false;
            }
            Invalidate();
            return true;
        }

        private void VerifyStartAtChkpt(Vector2 touchVec)
        {
            var currentStroke = _jieut.Strokes[_currentStokeIdx];
            var currentChkPt = currentStroke.ChkPts[_currentChkPtIdx];

            if ((touchVec - currentChkPt).MagSqr > CHKPT_RADIUS_SQR)
            {
                Console.Out.WriteLine("Didn't start from checkpoint!");
            }
        }

        public void Calculate(Vector2 touchVec)
        {
            _buffer = 0;
            //Console.Out.WriteLine(string.Format("Calulate called {0} times.", ++counter));

            // https://en.wikipedia.org/wiki/Vector_projection
            var projection = _chkPtVec * (_chkPtVec.Dot(touchVec) / _chkPtVec.MagSqr);
            var rejection = touchVec - projection;
            var errorSqr = rejection.MagSqr;

            if ((touchVec - _nextChkPt).MagSqr <= CHKPT_RADIUS_SQR)
            {
                Console.Out.WriteLine("Checkpoint reached!");
                ++_currentChkPtIdx;

                if (_currentChkPtIdx == _currentStroke.ChkPts.Count - 1)
                {
                    Console.Out.WriteLine("Stroke finished!");
                    _currentChkPtIdx = 0;

                    if (_currentStokeIdx == _jieut.Strokes.Count - 1)
                    {
                        _complete = true;
                    }
                    else
                    {
                        ++_currentStokeIdx;
                        _currentStroke = _jieut.Strokes[_currentStokeIdx];

                        _currentChkPt = _currentStroke.ChkPts[_currentChkPtIdx];
                        _nextChkPt = _currentStroke.ChkPts[_currentChkPtIdx + 1];
                        _chkPtVec = _nextChkPt - _currentChkPt;

                        DrawChkptCircles();
                    }
                }
                else
                {
                    _currentChkPt = _currentStroke.ChkPts[_currentChkPtIdx];
                    _nextChkPt = _currentStroke.ChkPts[_currentChkPtIdx + 1];
                    _chkPtVec = _nextChkPt - _currentChkPt;

                    DrawChkptCircles();
                }
            }
        }

        private void DrawChkptCircles()
        {
            _greenStrokesCanvas.DrawCircle(_currentChkPt.X, _currentChkPt.Y, 20, _chkptPaint);
            _greenStrokesCanvas.DrawCircle(_nextChkPt.X, _nextChkPt.Y, 20, _chkptPaint);
        }

        private void StatusCheck()
        {
            if (_complete)
            {
                Console.Out.WriteLine("Complete!");
            }
        }

        public void Reset()
        {
            _currentStokeIdx = 0;
            _currentChkPtIdx = 0;
            _currentStroke = _jieut.Strokes[_currentStokeIdx];
            _currentChkPt = _currentStroke.ChkPts[_currentChkPtIdx];
            _nextChkPt = _currentStroke.ChkPts[_currentChkPtIdx + 1];
            _chkPtVec = _nextChkPt - _currentChkPt;

            _greenStrokesCanvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            _redStrokesCanvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            _path.Reset();
            DrawChkptCircles();
            Invalidate();
        }
    }
}