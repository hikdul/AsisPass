using System.Text;
using System.Security.Cryptography;
using System;

namespace AsisPas.Helpers
{
    /// <summary>
    /// clase para generar key, hash y salt
    /// </summary>
    public class MD5
    {
        /// <summary>
        ///  encriptar usarndo el metodo MD5
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        protected string Encriptar(string texto, string key = "lineaSuperSecreta")
        {
            try
            {
                //esta llave deberia de generarse automaticamante e igual se tendria que almacenar
                // como es una llave que luego se pasa a un array de bytes pues se podria generar directamente asi!

                //llave para encriptar datos

                byte[] keyArray;

                byte[] Arreglo_a_Cifrar = UTF8Encoding.UTF8.GetBytes(texto);

                //Se utilizan las clases de encriptación MD5

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                hashmd5.Clear();

                //Algoritmo TripleDES
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();

                byte[] ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);

                tdes.Clear();

                //se regresa el resultado en forma de una cadena
                texto = Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
            return texto;
        }

        /// <summary>
        /// ahora desencriptamos usando MD5 y con la sal
        /// </summary>
        /// <param name="textoEncriptado"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string Desencriptar(string textoEncriptado, string key = "lineaSuperSecreta")
        {
            try
            {
                //string key = "qualityinfosolutions";
                byte[] keyArray;
                byte[] Array_a_Descifrar = Convert.FromBase64String(textoEncriptado);

                //algoritmo MD5
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                hashmd5.Clear();

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();

                byte[] resultArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);

                tdes.Clear();
                textoEncriptado = UTF8Encoding.UTF8.GetString(resultArray);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return textoEncriptado;
        }

        /// <summary>
        /// genera una clave aleatoria 
        /// </summary>
        /// <returns></returns>
        protected string GenerarKey()
        {

            try
            {
                Random rnd = new Random();
                String posibles = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                int longitud = posibles.Length;
                char lt;
                char lt2;
                char lt3;
                char lt4;
                int longitudnuevacadena = 11;
                string nuevacadena = "";
                string nuevacadena2 = "";
                string nuevacadena3 = "";
                string nuevacadena4 = "";
                for (int i = 0; i < longitudnuevacadena; i++)
                {
                    lt = posibles[rnd.Next(longitud)];
                    lt2 = posibles[rnd.Next(longitud)];
                    lt3 = posibles[rnd.Next(longitud)];
                    lt4 = posibles[rnd.Next(longitud)];
                    nuevacadena += lt.ToString();
                    nuevacadena2 += lt2.ToString();
                    nuevacadena3 += lt3.ToString();
                    nuevacadena4 += lt4.ToString();
                }
                String General = nuevacadena + nuevacadena2 + nuevacadena3 + nuevacadena4;
                return General;

            }
            catch
            {
                return null;
            }
        }
    }
}
