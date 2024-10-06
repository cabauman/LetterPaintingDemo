using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace DroidTouchDrawTest
{
    public class CustomView : View
    {
        public class Symbol
        {
            public List<Stroke> Strokes = new List<Stroke>();

            public Symbol(List<Stroke> strokes)
            {
                Strokes = strokes;
            }
        }


        public class Stroke
        {
            public List<Vector2> ChkPts = new List<Vector2>();

            public Stroke(List<Vector2> chkPts)
            {
                ChkPts = chkPts;
            }
        }


        private Symbol jieut;
        private List<Stroke> jieutStrokes = new List<Stroke>();
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


        public CustomView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
            testNum = 3;
        }

        private void Initialize()
        {
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            switch(testNum)
            {
                case 1:
                    TestInit1(w, h, oldw, oldh);
                    break;
                case 2:
                    TestInit2(w, h, oldw, oldh);
                    break;
                case 3:
                    TestInit3(w, h, oldw, oldh);
                    break;
            }
        }

        private Path path = new Path();
        private Paint paint = new Paint();
        private Bitmap pathBitmap;
        private Bitmap mPictureBitmap;
        private Bitmap mMaskBitmap;
        private Bitmap mBufferBitmap;
        private Canvas mBufferCanvas;
        private Paint mPaintSrcIn = new Paint();

        Bitmap bitmap;
        Bitmap maskBitmap;
        Bitmap filteredBitmap;
        Paint colorFilterPaint;
        Paint maskPaint;
        Paint testPaint;

        int testNum = 1;
        protected override void OnDraw(Canvas canvas)
        {
            switch(testNum)
            {
                case 1:
                    TestDraw1(canvas);
                    break;
                case 2:
                    TestDraw2(canvas);
                    break;
                case 3:
                    TestDraw3(canvas);
                    break;
            }
        }


        private void TestInit1(int w, int h, int oldw, int oldh)
        {
            mPaintSrcIn.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            mPictureBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.color_bg);
            mMaskBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.jieut);
            mPictureBitmap = Bitmap.CreateScaledBitmap(mPictureBitmap, w, w, true);
            mMaskBitmap = Bitmap.CreateScaledBitmap(mMaskBitmap, w, w, true);
            mBufferBitmap = Bitmap.CreateBitmap(mPictureBitmap.Width, mPictureBitmap.Height, Bitmap.Config.Argb8888);
            mBufferCanvas = new Canvas(mBufferBitmap);
            mBufferCanvas.DrawBitmap(mMaskBitmap, 0, 0, null);
            mBufferCanvas.DrawBitmap(mPictureBitmap, 0, 0, mPaintSrcIn);

            paint.StrokeWidth = 20f;
            paint.Color = Color.LawnGreen;
            paint.AntiAlias = true;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeJoin = Paint.Join.Round;
            paint.StrokeCap = Paint.Cap.Round;
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            //paint.SetColorFilter(new LightingColorFilter(Color.Red, 1));
        }

        private void TestDraw1(Canvas canvas)
        {
            mBufferCanvas.DrawPath(path, paint);
            //dump the buffer
            canvas.DrawBitmap(mBufferBitmap, 0, 0, null);
        }


        private void TestInit2(int w, int h, int oldw, int oldh)
        {
            bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.color_bg);
            maskBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.circle_mask);
            float[] src =
            {
                0, 0, 0, 0, 255,
                0, 0, 0, 0, 255,
                0, 0, 0, 0, 255,
                1, 1, 1, 0, 0,
            };
            ColorMatrix cm = new ColorMatrix(src);
            var filter = new ColorMatrixColorFilter(cm);
            maskPaint = new Paint();
            maskPaint.SetColorFilter(filter);
            maskPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));

            filteredBitmap = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
            mBufferCanvas = new Canvas(filteredBitmap);
            mBufferCanvas.DrawBitmap(bitmap, 0, 0, null);
            mBufferCanvas.DrawBitmap(maskBitmap, 0, 0, maskPaint);

            colorFilterPaint = new Paint();
            colorFilterPaint.SetColorFilter(new LightingColorFilter(0xffffff, 0x880000));
        }

        private void TestDraw2(Canvas canvas)
        {
            canvas.Save();
            mBufferCanvas.DrawPath(path, paint);
            //canvas.Scale(3, 3);
            canvas.DrawBitmap(bitmap, 0, 0, null);
            canvas.DrawBitmap(filteredBitmap, 0, 0, colorFilterPaint);
            canvas.Restore();
        }


        private void TestInit3(int w, int h, int oldw, int oldh)
        {
            //bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.jieut);
            maskBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.jieut);
            maskBitmap = Bitmap.CreateScaledBitmap(maskBitmap, w, w, true);
            //inverseMaskBitmap = Bitmap.CreateBitmap(maskBitmap.Width, maskBitmap.Height, Bitmap.Config.Argb8888);
            //CreateInversedBitmap();
            //inverseMaskBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.jieut_inverse);
            //inverseMaskBitmap = Bitmap.CreateScaledBitmap(inverseMaskBitmap, w, h, true);
            strokes = Bitmap.CreateBitmap(maskBitmap.Width, maskBitmap.Height, Bitmap.Config.Argb8888);
            strokesCanvas = new Canvas(strokes);
            strokes2 = Bitmap.CreateBitmap(maskBitmap.Width, maskBitmap.Height, Bitmap.Config.Argb8888);
            strokesCanvas2 = new Canvas(strokes2);
            chkptBitmap = Bitmap.CreateBitmap(maskBitmap.Width, maskBitmap.Height, Bitmap.Config.Argb8888);
            strokesCanvas2 = new Canvas(strokes2);

            float scale = w / 128f;
            paint.StrokeWidth = 15f * scale;
            paint.Color = Color.LawnGreen;
            paint.AntiAlias = true;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeJoin = Paint.Join.Round;
            paint.StrokeCap = Paint.Cap.Round;

            chkptPaint.StrokeWidth = 20;
            chkptPaint.Color = Color.AliceBlue;
            chkptPaint.AntiAlias = true;

            for(int i = 0;i < jieutStroke1Points.Count; ++i)
            {
                jieutStroke1Points[i] = jieutStroke1Points[i] * scale;
            }
            for(int i = 0; i < jieutStroke2Points.Count; ++i)
            {
                jieutStroke2Points[i] = jieutStroke2Points[i] * scale;
            }
            for(int i = 0; i < jieutStroke3Points.Count; ++i)
            {
                jieutStroke3Points[i] = jieutStroke3Points[i] * scale;
            }
            var stroke1 = new Stroke(jieutStroke1Points);
            var stroke2 = new Stroke(jieutStroke2Points);
            var stroke3 = new Stroke(jieutStroke3Points);
            jieutStrokes.Add(stroke1);
            jieutStrokes.Add(stroke2);
            jieutStrokes.Add(stroke3);
            jieut = new Symbol(jieutStrokes);

            currentStroke = jieut.Strokes[currentStokeIdx];
            currentChkPt = currentStroke.ChkPts[currentChkPtIdx];
            nextChkPt = currentStroke.ChkPts[currentChkPtIdx + 1];
            chkPtVec = nextChkPt - currentChkPt;

            DrawChkptCircles();
        }

        private void CreateInversedBitmap()
        {
            int length = maskBitmap.Width * maskBitmap.Height;
            int[] array = new int[length];
            maskBitmap.GetPixels(array, 0, maskBitmap.Width, 0, 0, maskBitmap.Width, maskBitmap.Height);
            for(int i = 0; i < length; i++)
            {
                array[i] = ~array[i];
            }
            inverseMaskBitmap.SetPixels(array, 0, maskBitmap.Width, 0, 0, maskBitmap.Width, maskBitmap.Height);
        }


        Bitmap inverseMaskBitmap;
        Canvas strokesCanvas;
        Bitmap strokes;
        Canvas strokesCanvas2;
        Bitmap strokes2;
        private void TestDraw3(Canvas canvas)
        {
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstOver));
            paint.Color = Color.Red;
            strokesCanvas2.DrawPath(path, paint);
            paint.Color = Color.LawnGreen;
            strokesCanvas.DrawPath(path, paint);

            // Painted strokes above are the destinations.
            // Now, color those destinations in the shape of the sources.
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));
            //strokesCanvas2.DrawBitmap(inverseMaskBitmap, 0, 0, paint);      // inverseMaskBitmap = source
            strokesCanvas.DrawBitmap(maskBitmap, 0, 0, paint);              // maskBitmap = source
            paint.SetXfermode(null);
            canvas.DrawBitmap(maskBitmap, 0, 0, paint);
            canvas.DrawBitmap(strokes2, 0, 0, paint);
            canvas.DrawBitmap(strokes, 0, 0, paint);
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            var touchX = e.GetX();
            var touchY = e.GetY();

            switch(e.Action)
            {
                case MotionEventActions.Down:
                    path.MoveTo(touchX, touchY);
                    VerifyStartAtChkpt(new Vector2(touchX, touchY));
                    break;
                case MotionEventActions.Move:
                    path.LineTo(touchX, touchY);
                    ++buffer;
                    if(buffer >= 2)
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


        private const float ERROR_THRESHOLD_SQR = 60 * 60;
        private const float CHKPT_RADIUS_SQR = 50 * 50;
        private int currentStokeIdx = 0;
        private int currentChkPtIdx = 0;
        private Stroke currentStroke;
        private Vector2 currentChkPt;
        private Vector2 nextChkPt;
        private Vector2 chkPtVec;
        private bool complete;
        private int buffer;
        private Paint chkptPaint = new Paint();
        private Bitmap chkptBitmap;

        private void VerifyStartAtChkpt(Vector2 touchVec)
        {
            var currentStroke = jieut.Strokes[currentStokeIdx];
            var currentChkPt = currentStroke.ChkPts[currentChkPtIdx];

            if((touchVec - currentChkPt).MagSqr > CHKPT_RADIUS_SQR)
            {
                Console.Out.WriteLine("Didn't start from checkpoint!");
            }
        }

        int counter = 0;
        public void Calculate(Vector2 touchVec)
        {
            buffer = 0;
            //Console.Out.WriteLine(string.Format("Calulate called {0} times.", ++counter));

            var projection = chkPtVec * (chkPtVec.Dot(touchVec) / (chkPtVec.MagSqr));
            var rejection = touchVec - projection;
            var errorSqr = rejection.MagSqr;

            if((touchVec - nextChkPt).MagSqr <= CHKPT_RADIUS_SQR)
            {
                Console.Out.WriteLine("Checkpoint reached!");
                ++currentChkPtIdx;

                if(currentChkPtIdx == currentStroke.ChkPts.Count - 1)
                {
                    Console.Out.WriteLine("Stroke finished!");
                    currentChkPtIdx = 0;

                    if(currentStokeIdx == jieut.Strokes.Count - 1)
                    {
                        complete = true;
                    }
                    else
                    {
                        ++currentStokeIdx;
                        currentStroke = jieut.Strokes[currentStokeIdx];

                        currentChkPt = currentStroke.ChkPts[currentChkPtIdx];
                        nextChkPt = currentStroke.ChkPts[currentChkPtIdx + 1];
                        chkPtVec = nextChkPt - currentChkPt;

                        DrawChkptCircles();
                    }
                }
                else
                {
                    currentChkPt = currentStroke.ChkPts[currentChkPtIdx];
                    nextChkPt = currentStroke.ChkPts[currentChkPtIdx + 1];
                    chkPtVec = nextChkPt - currentChkPt;

                    DrawChkptCircles();
                }
            }
        }

        private void DrawChkptCircles()
        {
            strokesCanvas.DrawCircle(currentChkPt.X, currentChkPt.Y, 20, chkptPaint);
            strokesCanvas.DrawCircle(nextChkPt.X, nextChkPt.Y, 20, chkptPaint);
        }

        private void StatusCheck()
        {
            if(complete)
            {
                Console.Out.WriteLine("Complete!");
            }
        }

        public void Reset()
        {
            currentStokeIdx = 0;
            currentChkPtIdx = 0;
            currentStroke = jieut.Strokes[currentStokeIdx];
            currentChkPt = currentStroke.ChkPts[currentChkPtIdx];
            nextChkPt = currentStroke.ChkPts[currentChkPtIdx + 1];
            chkPtVec = nextChkPt - currentChkPt;

            strokesCanvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            strokesCanvas2.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            path.Reset();
            DrawChkptCircles();
            Invalidate();
        }





        public struct Point2
        {
            public float X;
            public float Y;

            public Point2(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

    }
}