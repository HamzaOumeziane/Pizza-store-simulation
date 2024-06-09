namespace ThreadLivraison
{
    // Vous pouvez modifier le FACTEUR_ACCELERATION pour accélérer ou ralentir la simulation.
    // Le nombre de commandes, de cuisinier et de livreurs est aussi modifiable.
    public static class ConstantesSimulation
    {
        public const double FACTEUR_ACCELERATION = 200;
        public const double MILLIS_PAR_MINUTE = 60000.0 / FACTEUR_ACCELERATION;

        // Délai entre chaque commande (au téléphone)
        public const int DELAI_MINIMUM_COMMANDE = (int)(3 * MILLIS_PAR_MINUTE);
        public const int DELAI_MAXIMUM_COMMANDE = (int)(7 * MILLIS_PAR_MINUTE);

        // Temps de préparation d'une commande en cuisine
        public const int TEMPS_MINIMUM_PREPARATION = (int)(8 * MILLIS_PAR_MINUTE);
        public const int TEMPS_MAXIMUM_PREPARATION = (int)(14 * MILLIS_PAR_MINUTE);

        // Temps de déplacement pour se déplacer d'une case
        public const int TEMPS_DEPLACEMENT = (int)(1 * MILLIS_PAR_MINUTE);

        // Temps de paiement une fois la commande livrée
        public const int TEMPS_PAIEMENT = (int)(2 * MILLIS_PAR_MINUTE);

        // Temps maximal acceptable pour une commande
        public const int TEMPS_MAXIMAL_ATTENTE = (int)(45 * MILLIS_PAR_MINUTE);

        public const int NOMBRE_COMMANDES_LIVRAISON = 5;
        public const int DISTANCE_MAXIMALE_LIVRAISON = 10;

        // Nombre propres à la simulation
        public const int NOMBRE_COMMANDES = 80;
        public const int NOMBRE_CUISINIERS = 3;
        public const int NOMBRE_LIVREURS = 4;
    }
}
