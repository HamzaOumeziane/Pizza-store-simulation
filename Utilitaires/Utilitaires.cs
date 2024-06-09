namespace Utilitaires
{
    // creation de la librarie Utilitaires qui sera utilise par plusieurs classes dans le programme
    public class Utilitaires
    {
        // la methode calculerTemps permet de prendre en parametres un temps en millisecondes et le transformer en hh:mm:ss
       public static string calculerTemps(int nbMillisecondes)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(nbMillisecondes);
            return ts.ToString(@"hh\:mm\:ss");
        }
        
        // la methode calculerPosition permet de savoir la distance entre le lieu de la livraison et le lieu du depart (tout en incluant les axes X et Y)
        public static int calculerPosition(int x, int y)
        {
            return Math.Abs(x) + Math.Abs(y);
        }




    }
}