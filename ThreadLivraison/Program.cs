using ThreadLivraison;
using static ThreadLivraison.ConstantesSimulation;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections;
using Utilitaires;



/*
 * Nom : Hamza Oumeziane 
 * DA : 2226562
 * Pour le professeur : Mr Eric Wenaas
 * Dans le cadre du cours : Application Interactives (420-3GP-BB)
 * Ce travail pratique est une simulation d'une pizzeria. La pizzeria prend des commandes par telephone, puis livre les commandes de pizza aux clients  
 * de les avoir prepare. On utilise un facteur d'acceleration pour accelerer la simulation et savoir les fonctions de ce travail.
 */


// declaration des variables et des constantes :

Random random = new Random();
// la variable delaiCommande est un nombre aleatoire entre le delai minimum d'une commande et le delai maximum
int delaiCommande = random.Next(DELAI_MINIMUM_COMMANDE, DELAI_MAXIMUM_COMMANDE);
// Creation de la file concurrente lesCommandes pour traiter lescommandes de facon juste (premiere commande premiere preparee, First In First Out). Une file concurrente et pas normale pour assurer le ThreadSafe
ConcurrentQueue<Commande> lesCommandes = new ConcurrentQueue<Commande>();
// commandesPrepares est une variable de type commandeLivraison qui represente toutes les commandes prepares.
CommandeLivraison commandesPrepares = new CommandeLivraison();
// une constante qui represent 45 minutes en millisecondes
const int QUARANTE_CINQ_MINUTES_EN_MILI = 2700000;
// un compteur pour le nombre de livraisons hors delai
int livraisonsHorsDelai = 0;
// un chronometre pour connaitre le temps d'execution de la pizzeria
Stopwatch chronoTravail = new Stopwatch();


//Commencer le temps d'execution de la pizzeria car elle est ouverte
chronoTravail.Start();
Console.WriteLine("La pizzeria est ouverte");

// pour faire les commandes, le Thread threadCommande sert a creer des commandes et les annoncant par telephone
Thread threadCommande = new Thread(new ThreadStart(creerCommandes));
// commencer le thread des commandes
threadCommande.Start();

// les cuisiniers seront representes par un tableau de threads afin de laisser plusieurs cuisiniers travailler des commandes differentes en meme temps ,et pour gagner du temps
Thread[] cuisiniers = new Thread[NOMBRE_CUISINIERS];
for (int i = 0; i < NOMBRE_CUISINIERS; i++)
{
    // creer un cuisinier qui est un thread qui utilise la methode traiterCommande
    cuisiniers[i] = new Thread(new ParameterizedThreadStart(traiterCommande));
}

for (int i = 0; i < NOMBRE_CUISINIERS; i++)
{
    // commencer la fonction des cuisiniers qui est de preparer les commandes
    cuisiniers[i].Start(i);
}

// les livreurs sont representes par un tableau de threads afin de laisser plusieurs livreurs livrer des commandes differentes en meme temps ,et pour gagner du temps
Thread[] livreurs = new Thread[NOMBRE_LIVREURS];

for (int i = 0; i < NOMBRE_LIVREURS; i++)
{
    // creer un livreur qui est un thread qui utilise la methode livrerCommande
    livreurs[i] = new Thread(new ParameterizedThreadStart(livrerCommande));
}

for (int i = 0; i < NOMBRE_LIVREURS; i++)
{
    // commencer la fonction des livreur qui est de livrer les commandes
    livreurs[i].Start(i);    
}

// On affiche les stats du programme
afficherStats();

// la methode livrerCommande qui sera utilise par les threads livreurs, chaque commande preparee soit livree de ce processus, elle prend en parametre le numero du livreur
void livrerCommande(object? valeurObjet)
{
    // la dureeDistance est la duree de la distance que le livraison fait entre le depart et l'arrive
    int dureeDistance = 0;
    // l'objet en parametre est de type int 
    int valeur = (int)valeurObjet;
    // la distance retour est la distance entre la derniere commande du livreur avant de retourner a la pizzeria et la position de la pizzeria (X=0 et Y=0)
    int distanceRetour = 0;
    // une list de type commande, est les livraisons du livreur d'un seul voyage
    List<Commande> livraisons = new List<Commande>();
    // si les cuisiniers travaillent encore et ils existent des commandesPrepares, les livreurs livrent
    while (!verifierCuisiniersMorts() || !commandesPrepares.estVide())
    {
        // le livreur obtient la liste des commandes a livrer en un seul voyage, le choix des commandes est precisé dans la methode obtenirCommande() 
        livraisons = commandesPrepares.obtenirCommande();
        // s'ils n'y a pas des commandes a livrer, on reessaye de chercher
        if (livraisons.Count == 0)
        {
            Thread.Sleep(1);
        }
        else
        {
            // afficher les commandes qu'un livreur va livreur
            Console.WriteLine($"Le livreur #{(valeur + 1)} va livré les commandes : ");
            afficherCommandeLivree(livraisons);

            // on commence a livrer chaque commande, une par une
            for (int i = 0; i < livraisons.Count; i++)
            {
                Console.WriteLine($"Le livreur #{(valeur + 1)} va livre la commande : #{livraisons[i].Numero}");
                // si le livreur va livrer la premiere commande, donc dureeDistance est la duree de distance entre le lieu de la pizzeria et le lieu de livraison + le temps de paiement
                if (i == 0)
                {
                    dureeDistance = (Utilitaires.Utilitaires.calculerPosition(livraisons[i].Destination.X, livraisons[i].Destination.Y) * TEMPS_DEPLACEMENT) + TEMPS_PAIEMENT;
                }
                else
                {
                    // sinon, la dureeDistance est entre le lieu de la premiere livraison et la livraison actuelle
                    dureeDistance = (Math.Abs(livraisons[i].Destination.X - livraisons[i].Destination.X) + Math.Abs(livraisons[0].Destination.Y - livraisons[i].Destination.Y) * TEMPS_DEPLACEMENT) + TEMPS_PAIEMENT;
                }
                // le livreur fait la livraison
                Thread.Sleep((int)(dureeDistance));
                // la commande est livree, donc le chrono qui reprensente le temps de livraison de la commande est arrete
                livraisons[i].setEstLivree(true);
                Console.WriteLine($"Le livreur #{(valeur + 1)} a livré la commande : #{livraisons[i].Numero}. Temps : {Utilitaires.Utilitaires.calculerTemps(livraisons[i].TempsLivraison)}");
                // si le temps de la livraison depasse 45minutes, on la considere comme une livraison hors delai
                if (livraisons[i].TempsLivraison > QUARANTE_CINQ_MINUTES_EN_MILI)
                {
                    livraisonsHorsDelai++;
                }
                // pour retourner a la pizzeria, le livreur prend la distance entre la derniere livraison et le lieu de la pizzeria
                if(i == livraisons.Count - 1)
                {
                    distanceRetour = (Utilitaires.Utilitaires.calculerPosition(livraisons[livraisons.Count - 1].Destination.X, livraisons[livraisons.Count - 1].Destination.Y) * TEMPS_DEPLACEMENT);
                }
            }
            // on enleve la commande livree des livraisons, car elle est deja livree
            for (int k = 0; k < livraisons.Count; k++)
            {
                livraisons.Remove(livraisons[k]);
            }
            // le livreur retourne a la pizzeria
            Thread.Sleep((int)(distanceRetour));

            Console.WriteLine($"Le livreur #{(valeur + 1)} est de retour à la pizzeria");
        }
    }
}
    
   
// methode creerCommande qui sera utilise par le thread des commandes, chaque commande sera cree entre 3 et 7 minutes
void creerCommandes()
{
    // depuis un nombre de commande precis, on cree des commandes
    for (int i = 0; i < NOMBRE_COMMANDES; i++)
    {
        // le temps de preparations des commandes sont cree de facon aleatoire entre 8 minutes et 14 minutes
        int tempsPreparation = random.Next(TEMPS_MINIMUM_PREPARATION, TEMPS_MAXIMUM_PREPARATION);
        // la position des commandes sont aussi cree de facon aleatoire entre -10 et 10 sur les axes X et Y
        int positionX = random.Next(-10, 11);
        int positionY = random.Next(-10, 11);
        // faire une commande entre 3 et 7 minutes (delai commande de facon aleatoire)
        Thread.Sleep((int)(delaiCommande));
        // une commande est cree avec un nombre, une position, un temps de preparation et un chronometre qui commence pour representer le temps de livraison a la fin 
        Commande nouvelleCommande = new Commande(i + 1, new Position(positionX, positionY), tempsPreparation, new Stopwatch());
        // afficher la commande et ses caracteristiques : numero, destination, temps de preparation
        Console.WriteLine(nouvelleCommande.ToString());
        // a chaque fois qu'un commande est cree, donc elle est ajoutee a la file
        lesCommandes.Enqueue(nouvelleCommande);

    }
}

// methode traiterCommande qui sera utilise par les cuisiniers, est une methode qui permet a preparer les commandes selon leurs temps de preparation. Elle prend en parametre le numero du cuisinier pour distinguer entre eux
void traiterCommande(object? valeur)
{
    // a chaque fois qu'un commande est declare ou y'a deja des commandes qui existent, la cuisine est ouverte
    while (threadCommande.IsAlive == true || lesCommandes.IsEmpty == false)
    {
        Commande uneCommande;
        int tempsPreparation = 0;
        // Si on peut prendre la premiere commande de la file, on commence a la preparer
        if (lesCommandes.TryDequeue(out uneCommande))
        {
            // le cuisinier commence a preparer la commande
            Console.WriteLine($"Le cuisinier #{((int)(valeur) + 1)} commence la commande #{uneCommande.Numero} {Utilitaires.Utilitaires.calculerTemps((int)(uneCommande.TempsPreparation * FACTEUR_ACCELERATION))}");
            // le cuisinier est en train de preparer la commande
            Thread.Sleep((int)(uneCommande.TempsPreparation));
            // le cuisinier a termine de preparer la commande, du coup cette commande va etre ajoute aux commandes prepares
            Console.WriteLine($"Le cuisinier #{((int)(valeur) + 1)} a terminé la commande #{uneCommande.Numero}");
            commandesPrepares.Ajouter(uneCommande);
        }
        else
        {
            // si on ne peut pas prendre la premiere commande de la file, on reessaye encore de la prendre
            Thread.Sleep(1);
        }


    }

}

// une methode d'affichade des commandes livrees par les livreurs en prenant en parametres la liste des commandes livrees
void afficherCommandeLivree(List<Commande> cmd)
{
    foreach (Commande c in cmd)
    {
        Console.WriteLine($"           #{c.Numero} à {c.Destination}");
    }
}

// la methode afficherStats qui sert a donner les statistiques de la journee : temps d'attente moyen d'une commande, le temps du travail et le nombre de commandes qui sont hors delai
void afficherStats()
{
    while (true)
    {
        // les statistiques seront affiches que lorsque les livreurs sont tous retournes a la pizzeria, donc a la fin du programme
        if (verifierLivreursMorts())
        {
            // afficher temps d'attente moyen d'une commande, le temps du travail et le nombre de commandes qui sont hors delai
            Console.WriteLine($"Temps d'attente moyen : {calculerTempsAttenteMoyen()}");
            Console.WriteLine($"Temps d'exécution : {Utilitaires.Utilitaires.calculerTemps((int)((chronoTravail.ElapsedMilliseconds) * FACTEUR_ACCELERATION))}");
            Console.WriteLine($"Nombre de commandes hors délai : {livraisonsHorsDelai}");
            break;
        }
    }
}

// la methode calculerTempsAttenteMoyen sert a calculer le temps d'attente moyen de tous les livraisons, en divisant le temps de livraison global (de toutes les commandes) et le diviser par le nombre des commandes, puis le transformer en hh:mm:ss
string calculerTempsAttenteMoyen()
{
    return Utilitaires.Utilitaires.calculerTemps((int)((Commande.tempsLivraisonGlobal / NOMBRE_COMMANDES)));
}

// methode verifierCuisiniersMorts sert a preciser si la cuisine est ouverte ou non, si un des cuisiniers travaille on retourne false (thread pas mort) et si tous ont fini leur travail (prepare toutes les commandes) on retourne true (thread mort) 
bool verifierCuisiniersMorts()
{
    foreach(Thread cuis in cuisiniers)
    {
        if (cuis.IsAlive)
        {
            return false;
        }
    }
    return true;
}

// methode verifierLivreursMorts sert a preciser si les livreurs ont fini leur travaille ou non, si un des livreurs livre enconre on retourne false (thread pas mort) et si tous ont fini leur livraison (toutes les commandes sont livree) on retourne true (thread mort) 
bool verifierLivreursMorts()
{
    foreach(Thread liv in livreurs)
    {
        if(liv.IsAlive)
        {
            return false;
        }
    }
    return true;
}

