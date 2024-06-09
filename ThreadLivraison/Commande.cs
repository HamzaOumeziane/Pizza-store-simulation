using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilitaires;
using static ThreadLivraison.ConstantesSimulation;

namespace ThreadLivraison
{
    internal class Commande
    {
        // un attribut booleen _estLivree qui sert a preciser si la commande est livree ou non
        private bool _estLivree;
        // un attribut Stopwatch qui sert a chronometrer la duree de livraison de la commande
        private Stopwatch _stopwatch;
        // un attribut statique qui est le temps de livraison de toutes les commandes
        public static int tempsLivraisonGlobal = 0;

        // la methode SetEstLivree qui sert a modifier l'etat de la commande (livree ou pas encore)
        public void setEstLivree(bool estLivree)
        {
            _estLivree = estLivree;
            // a chaque fois qu'une commande est livree, le temps de livraison global augmente du temps de livraison de la commande
            tempsLivraisonGlobal += TempsLivraison;
            // a chaque fois qu'une commande est livree, on arrete le chronometre de la duree de livraison
            _stopwatch.Stop();
            
        }

        // l'attribut Numero qui represente le numero de la commande
        public int Numero
        {
            get;
            set;
        }

        // l'attribut destination qui est de type Position qui represente les dimensions X et Y de la destination de la commande
        public Position Destination
        {
            get;
            set;
        }

        //l'attribut tempsPreparation qui represente le temps qu'un cuisinier prend pour preparer la commande
        public int TempsPreparation
        {
            get;
            set;
        }

        // l'attribut TempspLivraison qui represente le temps de la livraison de la commande depuis qu'elle est declare par le telephone
        public int TempsLivraison
        {
            // methode getTempsLivraison qui sert a retourner le chronometre de la duree de livraison de la commande en millisecondes
            get
            {
                return (int)(_stopwatch.ElapsedMilliseconds * FACTEUR_ACCELERATION);
            }
        }

        
        // constructeur de la classse commande
        public Commande(int numero, Position destination, int tempsPrep, Stopwatch sw)
        {
            Numero = numero;
            Destination = destination;
            TempsPreparation = tempsPrep;
            _stopwatch = sw;
            // lorsqu'on declare une commande, on commence le chrono de la duree de livraison
            sw.Start();
            
        }

        // la methode toString qui sera utilise lorsqu'une commande est appele par le telephone
        public string ToString()
        {
            return $"Commande #{Numero} ==> Temps de préparation : {Utilitaires.Utilitaires.calculerTemps((int)(TempsPreparation * FACTEUR_ACCELERATION))}, Destination : {Destination.ToString()}";
        }
    }
        

   }

