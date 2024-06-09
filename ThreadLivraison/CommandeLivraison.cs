using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilitaires;

namespace ThreadLivraison
{
    // creation de la classe commandeLivraison
    internal class CommandeLivraison
    {
        // un attribut qui est une liste de toutes les commandes prepares
        private List<Commande> _commandes = new List<Commande>();
        // un attribut verrou de blocage pour assurer le ThreadSafe
        private object _lock = new object();

        // une methode qui sert a ajouter une commande aux commandes prepares
        public void Ajouter(Commande cmd)
        {
            lock (_lock)
            {
                _commandes.Add(cmd);
            }
        }

        // une methode qui permet de savoir si il existe des commandes prepares ou pas, faux si il y en a et vrai s'il n'y en a pas
        public bool estVide()
        {
            lock (_lock)
            {
                return (_commandes.Count == 0);
            }
        }
        
        // la methode obtenirCommande qui sert a retourner les commandes prepares que le livreur prend en un seul voyage
        public List<Commande> obtenirCommande()
        {
            // la liste livraison qu'on va retourner (les commandes prepares en un seul voyage)
            List<Commande> livraison = new List<Commande>();
            
            // si un thread veut prendre des commandes, l'autre threads prend des commandes differents, on evite que deux threads prennent les memes commandes
            lock (_lock)
            {
                // s'il existe des commandes prepares
                if(_commandes.Count != 0)
                {
                    // la variable depart represente la premiere commande a livrer dans le voyage
                    Position depart = _commandes[0].Destination;
                    for (int i = 0; i < _commandes.Count; i++)
                    {
                        // si c'est la premiere commande, on l'ajoute dans la liste livraison
                        if (i == 0)
                        {
                            livraison.Add(_commandes[i]);
                        }
                        else
                        {
                            // si ce n'est pas la premiere commande, on compare la distance entre cette commande et la premiere commande, si la distance entre eux est moins que 10
                            // et que la liste des livraisons contient moins de 5 commandes, on l'ajoute a la liste, sinon on l'ajoute pas
                            int distance = Math.Abs(depart.X - _commandes[i].Destination.X) + Math.Abs(depart.Y - _commandes[i].Destination.Y);
                            if ((distance < 10) && (livraison.Count < 5))
                            {
                                livraison.Add(_commandes[i]);
                            }
                        }
                    }

                    // les commandes prepares qui sont dans les livraisons on les enleve des commandes prepares.
                    for (int j = 0; j < livraison.Count; j++)
                    {
                        _commandes.Remove(livraison[j]);
                    }
                    
                }
                return livraison;


            }
        }
    }
}
