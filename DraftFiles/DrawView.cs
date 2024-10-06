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
    public class DrawView : View
    {
        public class Stroke
        {
            public Path Path { get; set; }
            public Paint Paint { get; set; }

            public Stroke(uint color)
            {
                Path = new Path();
                Paint = new Paint();
                Paint.Color = new Color((int)color);
                Paint.AntiAlias = true;
                Paint.SetStyle(Paint.Style.Stroke);
                Paint.StrokeJoin = Paint.Join.Round;
                Paint.StrokeCap = Paint.Cap.Round;
            }
        }

        private List<Stroke> strokes = new List<Stroke>();
        private Stroke paintStroke;
        private Stroke errorStroke;
        private Stroke currentStroke;
        private uint errorColor = 0xFF660000;
        private uint paintColor = 0xFF006600;
        private Paint canvasPaint;
        
        private Canvas drawCanvas;
        private Bitmap canvasBitmap;
        private Bitmap letterBitmap;
        private float width;
        private float height;
        private float validTouchRadiusSqr = 40f;
        private float oldX, oldY;
        private float scaleFactor;
        private bool isErrorColor;


        public DrawView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public DrawView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            paintStroke = new Stroke(paintColor);
            errorStroke = new Stroke(errorColor);
            currentStroke = paintStroke;

            canvasPaint = new Paint();
            canvasPaint.Dither = true;
        }

        private Bitmap maskBitmap;
        private Paint maskPaint;
        private Bitmap filteredBitmap;
        private Paint colorFilterPaint;
        private Bitmap pathBitmap;
        private void Initialize2()
        {

        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            width = height = w;
            canvasBitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
            drawCanvas = new Canvas(canvasBitmap);

            letterBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.jieut);
            letterBitmap = Bitmap.CreateScaledBitmap(letterBitmap, w, w, true);
            scaleFactor = w / 128f;
            currentStroke.Paint.StrokeWidth = 15f * scaleFactor;
            validTouchRadiusSqr *= scaleFactor;
        }

        protected override void OnDraw(Canvas canvas)
        {
            //RectF bounds = new RectF();
            //currentStroke.Path.ComputeBounds(bounds, true);
            //pathBitmap = Bitmap.CreateBitmap((int)bounds.Width(), (int)bounds.Height(), Bitmap.Config.Argb8888);
            //Canvas pathCanvas = new Canvas(pathBitmap);
            //Canvas c = new Canvas(filteredBitmap);
            //c.DrawBitmap(pathBitmap, 0, 0, null);
            //c.DrawBitmap(maskBitmap, 0, 0, maskPaint);

            var y = (canvas.Height / 2) - (letterBitmap.Height / 2);
            //canvas.Save();
            //canvas.DrawBitmap(letterBitmap, 0, y, canvasPaint);
            //canvas.DrawBitmap(pathBitmap, 0, y, null);
            //canvas.DrawBitmap(filteredBitmap, 0, 0, colorFilterPaint);
            //canvas.Restore();

            canvas.DrawBitmap(letterBitmap, 0, y, canvasPaint);
            canvas.DrawBitmap(canvasBitmap, 0, y, canvasPaint);

            foreach(var s in strokes)
            {
                canvas.DrawPath(s.Path, s.Paint);
            }
            //canvas.DrawPath(currentStroke.Path, currentStroke.Paint);

            //var paint = new Paint();
            //paint.Color = Color.Gray;
            //foreach(var p in points)
            //{
            //    canvas.DrawPoint(p.X, p.Y, paint);
            //}
        }

        Point2[] points = new Point2[20];
        double[] rand = new double[20];
        public override bool OnTouchEvent(MotionEvent e)
        {
            var touchX = e.GetX();
            var touchY = e.GetY();
            var brushRadius = currentStroke.Paint.StrokeWidth * 0.5f;

            switch(e.Action)
            {
                case MotionEventActions.Down:
                    //for(int i = 0; i < 20; i++)
                    //{
                    //    rand[i] = GetRand();
                    //}
                    ValidateTest(touchX, touchY);
                    currentStroke.Path.MoveTo(touchX, touchY);

                    //for(int i = 0; i < 20; i++)
                    //{
                    //    float x = touchX + (float)rand[i] * brushRadius;
                    //    float y = touchX + (float)rand[i] * brushRadius;
                    //    points[i] = new Point2(x, y);
                    //}
                    break;
                case MotionEventActions.Move:
                    bool strokeChanged = ValidateTest(touchX, touchY);
                    if(strokeChanged)
                    {
                        currentStroke.Path.MoveTo(touchX, touchY);
                    }
                    else
                    {
                        currentStroke.Path.LineTo(touchX, touchY);
                    }

                    //for(int i = 0; i < 20; i++)
                    //{
                    //    float x = touchX + (float)rand[i] * brushRadius;
                    //    float y = touchX + (float)rand[i] * brushRadius;
                    //   points[i] = new Point2(x, y);
                    //}
                    break;
                case MotionEventActions.Up:
                    //drawCanvas.DrawPath(currentStroke.Path, currentStroke.Paint);
                    //currentStroke.Path.Reset();
                    break;
                default:
                    return false;
            }
            Invalidate();
            return true;
        }

        Random rng = new Random(); //reuse this if you are generating many
        private double GetRand()
        {
            double u1 = 1.0 - rng.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            var mean = 0d;
            var stdDev = 1d;
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }

        //18, 25 *
        //100, 25 *

        //63, 32 *
        //56, 68
        //40, 88
        //19, 98 *

        //67, 60 *
        //88, 88
        //108, 98 *

        private bool ValidateTest(float x, float y)
        {
            Vector2 point1 = new Vector2(18, 25) * scaleFactor;
            Vector2 point2 = new Vector2(100, 25) * scaleFactor;
            Vector2 chkptVector = point2 - point1;
            Vector2 touchVector = new Vector2(x, y) - point1;
            var error = GetError(chkptVector, touchVector);
            //var error = point1.X - x;
            //if(error <= 900f)
            //{
            //    var projection = chkptVector * (chkptVector.Dot(touchVector) / (chkptVector.MagSqr));
            //    var rejection = touchVector - projection;
            //    error = rejection.MagSqr;
            //}

            bool strokeChanged = false;
            if(error > 30f)
            {
                if(!isErrorColor)
                {
                    isErrorColor = true;
                    strokeChanged = true;
                    currentStroke = new Stroke(errorColor);
                    currentStroke.Paint.StrokeWidth = 15f * scaleFactor;
                    strokes.Add(currentStroke);
                }
            }
            else
            {
                if(isErrorColor)
                {
                    isErrorColor = false;
                    strokeChanged = true;
                    currentStroke = new Stroke(paintColor);
                    currentStroke.Paint.StrokeWidth = 15f * scaleFactor;
                    strokes.Add(currentStroke);
                }
            }

            return strokeChanged;
        }







        private float GetError(Vector2 chkptVec, Vector2 touchVec)
        {
            //float normalizedX = x / width;
            //float normalizedY = y / height;
            //float deltaSqrX = (x-oldX) * (x-oldX);
            //float deltaSqrY = (x-oldY) * (x-oldY);
            //float deltaSqr = deltaSqrX + deltaSqrY;
            //if(deltaSqr <= validTouchRadiusSqr)
            //{

            //}

            //Vector2 point1 = new Vector2(18, 25) * scaleFactor;
            //Vector2 point2 = new Vector2(100, 25) * scaleFactor;
            //Vector2 chkptVector = point2 - point1;
            //Vector2 touchVector = new Vector2(x, y);

            Vector2 chkptVecNormal = new Vector2(-chkptVec.Y, chkptVec.X);
            chkptVecNormal = chkptVecNormal / chkptVecNormal.Magnitude;
            Vector2 errorVector = touchVec - chkptVec;
            float error = Math.Abs(errorVector.Dot(chkptVecNormal));

            return error;
        }

        public void Reset()
        {
            strokes.Clear();
            Invalidate();
        }






        //private void Validate()
        //{
        //    ImageView imageView = new ImageView(Context);
        //    imageView.SetImageResource(Resource.Drawable.Icon);
        //    Bitmap img = BitmapFactory.DecodeResource(Resources, Resource.Drawable.jieut);

        //    //System.Drawing.Bitmap img = new System.Drawing.Bitmap("");
        //    for(int i = 0; i < img.Width; i++)
        //    {
        //        for(int j = 0; j < img.Height; j++)
        //        {
        //            var pixel = img.GetPixel(i, j);
        //            var alpha = Color.GetAlphaComponent(pixel);

        //            if(alpha > 10)
        //            {
        //                //return true;
        //            }
        //        }
        //    }
        //}





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