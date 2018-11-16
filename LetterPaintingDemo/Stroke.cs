using System.Collections.Generic;

namespace LetterPaintingDemo
{
    public class Stroke
    {
        public List<Vector2> ChkPts = new List<Vector2>();

        public Stroke(List<Vector2> chkPts)
        {
            ChkPts = chkPts;
        }
    }
}