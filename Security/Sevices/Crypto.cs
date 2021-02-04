using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Security.Sevices
{
    public class Crypto
    {
        private static string encryptKey = "DEFAULT";


        public static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        public static string DecryptString(string cipherText, string Key, string IV)
        {
            return DecryptStringFromBytes(Convert.FromBase64String(cipherText), Encoding.UTF8.GetBytes(Key), Encoding.UTF8.GetBytes(IV));
        }

        public static string EncryptString(string text, string Key, string IV)
        {
            return Convert.ToBase64String(EncryptStringToBytes(text, Encoding.UTF8.GetBytes(Key), Encoding.UTF8.GetBytes(IV)));
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }

        static public string ToBase64(string cadena)
        {
                //Arreglo de bytes donde guardaremos la llave
                byte[] keyArray;
                //Arreglo de bytes donde guardaremos el texto que vamos a encriptar
                byte[] Arreglo_a_Cifrar =
                UTF8Encoding.UTF8.GetBytes(cadena);
                //se utilizan las clases de encriptación provistas por el Framework Algoritmo MD5
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                //se guarda la llave para que se le realice hashing
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(encryptKey));
                hashmd5.Clear();
                //Algoritmo 3DAS
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                //Se empieza con la transformación de la cadena
                ICryptoTransform cTransform = tdes.CreateEncryptor();
                //Arreglo de bytes donde se guarda la cadena cifrada
                byte[] ArraycExpresionado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);
                tdes.Clear();
                //Se regresa el cExpresionado en forma de una cadena

                byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToBase64String(ArraycExpresionado, 0, ArraycExpresionado.Length));
                return System.Convert.ToBase64String(toEncodeAsBytes);
        }
    }
}
