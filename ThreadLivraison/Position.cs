using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadLivraison
{
    // creation de la classe Position
    internal class Position
    {
        // l'attribut X qui represente la coordonnee cartesienne de la commande sur l'axe X
        public int X
        {
            get;
            set;
        }

        // l'attribut Y qui represente la coordonnnee cartesienne de la commande sur l'axe Y
        public int Y 
        { 
            get; 
            set; 
        }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    


    }
}
