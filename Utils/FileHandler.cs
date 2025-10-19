using System;
using System.IO;

namespace Finance.Utils
{
    public static class FileHandler
    {
        // Utiliser LocalApplicationData au lieu de BaseDirectory pour eviter les problemes de permissions
        private static readonly string InternalStoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Finance Manager",
            "UploadedFiles"
        );

        /// <summary>
        /// Retourne le chemin du dossier de stockage interne et le cree s'il n'existe pas
        /// </summary>
        public static string GetInternalStoragePath()
        {
            if (!Directory.Exists(InternalStoragePath))
            {
                Directory.CreateDirectory(InternalStoragePath);
            }
            return InternalStoragePath;
        }

        /// <summary>
        /// Copie un fichier depuis la source vers le dossier interne de l'application
        /// </summary>
        /// <param name="sourceFilePath">Chemin du fichier source</param>
        /// <returns>Chemin du fichier copie dans le dossier interne</returns>
        public static string CopyFileToInternalStorage(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Le fichier source n'existe pas.", sourceFilePath);
            }

            string internalFolder = GetInternalStoragePath();
            string fileName = Path.GetFileName(sourceFilePath);
            
            // Generer un nom unique si un fichier avec le meme nom existe deja
            string destinationPath = Path.Combine(internalFolder, fileName);
            int counter = 1;
            while (File.Exists(destinationPath))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                fileName = $"{fileNameWithoutExtension}_{counter}{extension}";
                destinationPath = Path.Combine(internalFolder, fileName);
                counter++;
            }

            File.Copy(sourceFilePath, destinationPath, false);
            return destinationPath;
        }

        public static string SaveFile(string sourceFilePath, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            var fileName = Path.GetFileName(sourceFilePath);
            var destinationPath = Path.Combine(destinationFolder, fileName);

            File.Copy(sourceFilePath, destinationPath, true);

            return destinationPath;
        }
    }
}