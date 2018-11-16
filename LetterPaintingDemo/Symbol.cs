using System.Collections.Generic;

namespace LetterPaintingDemo
{
    public class Symbol
    {
        public List<Stroke> Strokes = new List<Stroke>();

        public Symbol(List<Stroke> strokes)
        {
            Strokes = strokes;
        }
    }
}