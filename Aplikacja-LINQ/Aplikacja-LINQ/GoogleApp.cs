using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplikacja_LINQ
{
     public class GoogleApp
    {

        public string Name { get; set; }
        public Category Category { get; set; }  // enum kategorii
        public decimal Rating { get; set; }
        public int Reviews { get; set; }
        public string Size { get; set; }
        public string Installs { get; set; }
        public Type Type { get; set; }  // typ jako enum

        public string Price { get; set; }


        public string ContentRating { get; set; }


        public List<string> Genres { get; set; }   // lista gatunków jako string

        public DateTime LastUpdates { get; set; }   // ostatnia aktualizacja

        public override string ToString()
        {
            return $"{(Name.Length > 25 ? Name.Substring(0, 25) + "..." : Name),-28} | " +
                $"{Category,-20} | " +
                $"{Rating,-3} | " +
                $"{Reviews,-10} | " +
                $"{Type,-4} | " +
                $"{Price,-7} | " +
                $"{LastUpdates.ToShortDateString(),-10} | " +
                $"{string.Join(",", Genres)}";
        }


    }
}
