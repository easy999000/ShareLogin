using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CoreWeb.code.HQShareLogin
{

    public class RSAHelper
    {
        private string privateKey = "<RSAKeyValue><Modulus>0wE26IHp4U9OLtPhJ+fT8ej6aWORFP8pd++MjUuhkQQm/zhcImbxQbjxtSAftz+kkDwGDFJpSldQPyigOGcUx7PofTc6VhiFik9E9SsxV9n0iEEtqUndDfmBJfPAWt+4UDMwKakgZqFoapDuwjKlTErFvKCyKCs+qN9OZvZwKWk=</Modulus><Exponent>AQAB</Exponent><P>8Ei6NIsZtgV3DQjuGHfGLS6o1O+IUXxzjqLxdMm77yhEPUxR9YPIxODJ2VVTddXSAHxViJJt30yJ7JhVz6cpQw==</P><Q>4M49NrmalgVQFMsea2RMB1qN8fAPfIw5G9q9hzsLcWSCmkeRRIQlvPYflVEKAYKiDVVzENETbnnduFXWBABx4w==</Q><DP>t+JQbemN0Zi5FQaif6MZzHYKynpNTl75aE0Wj5Pa+RlNr8N6bXNe8Bw/HM2Jw4HQ5oJASvYUk3DVlHS4JuP8VQ==</DP><DQ>lT62iv9brp9mU/epgVh71SH8PJPIZEJfo6tryjyb0zMMNcqvmZI1z6aCv0mm3+vPFBUXqCF1yhFj7n4l8FAvSw==</DQ><InverseQ>flrvgxHvf4l+fdymEVDgKjsfGqshOpppoNgZj9kpeWBto3o8z++Ki6eSLQT3nVnpx2QCZeTWkxTED4nhSLKscw==</InverseQ><D>cQTCg1Eqk7sltmFYxUYgOP/AOPjSufteG9acYwYymPkvZh6rAuY+rSRBmvGE62NUYskzuB/gM6iG2/2HrA5SixfNgCvZ+nsK+kX5pzQRsYdD71ViQW0hOanXwj45I2zHRgBiuTtCUP0fs5pISmQkaeJkDL5pO2l+wvlgl+wunj0=</D></RSAKeyValue>";
        private string publicKey = "<RSAKeyValue><Modulus>0wE26IHp4U9OLtPhJ+fT8ej6aWORFP8pd++MjUuhkQQm/zhcImbxQbjxtSAftz+kkDwGDFJpSldQPyigOGcUx7PofTc6VhiFik9E9SsxV9n0iEEtqUndDfmBJfPAWt+4UDMwKakgZqFoapDuwjKlTErFvKCyKCs+qN9OZvZwKWk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public RSAHelper(string PrivateKey, string PublicKey)
        {
            privateKey = PrivateKey;
            publicKey = PublicKey;
        }
        public string Encrypt(string encrypt)
        {
            byte[] data = Encoding.UTF8.GetBytes(encrypt);
            var EncryptData = Encrypt(data );
            return Convert.ToBase64String(EncryptData);

        }
        public byte[] Encrypt(byte[] data)
        { 
            var EncryptData = Encrypt(data, RSAEncryptionPadding.Pkcs1);
            return EncryptData;

        }

        public byte[] Encrypt(byte[] encryptBytes, RSAEncryptionPadding padding)
        {
            using (var rsa = RSA.Create())
            {

                FromXmlString(rsa, publicKey);

                var maxBlockSize = GetMaxBlockSize(rsa, padding);

                if (encryptBytes.Length <= maxBlockSize)
                {
                    var @bytes = rsa.Encrypt(encryptBytes, padding);
                    return @bytes;
                }

                using (var memoryStream = new MemoryStream(encryptBytes))
                {
                    using (var readStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[maxBlockSize];

                        int blockSize = memoryStream.Read(buffer, 0, maxBlockSize);

                        while (blockSize > 0)
                        {
                            var blockByte = new byte[blockSize];

                            Array.Copy(buffer, 0, blockByte, 0, blockSize);

                            var encrypts = rsa.Encrypt(blockByte, padding);

                            readStream.Write(encrypts, 0, encrypts.Length);

                            blockSize = memoryStream.Read(buffer, 0, maxBlockSize);
                        }

                        return readStream.ToArray();
                    }
                }
            }
        }


        public string Decrypt(string decrypt)
        {
            var EncryptData = Convert.FromBase64String(decrypt);
            var data = Decrypt(EncryptData );
            return Encoding.UTF8.GetString(data);
        }
        public byte[] Decrypt(byte[] decrypt)
        { 
            var data = Decrypt(decrypt, RSAEncryptionPadding.Pkcs1);
            return data;
        }

        public byte[] Decrypt(byte[] decryptBytes, RSAEncryptionPadding padding)
        {
            using (var rsa = RSA.Create())
            {
                FromXmlString(rsa, privateKey);

                var maxBlockSize = rsa.KeySize / 8;

                if (decryptBytes.Length <= maxBlockSize)
                {
                    var @bytes = rsa.Decrypt(decryptBytes, padding);
                    return @bytes;
                }

                using (var memoryStream = new MemoryStream(decryptBytes))
                {
                    using (var readStream = new MemoryStream())
                    {
                        var buffer = new byte[maxBlockSize];

                        var blockSize = memoryStream.Read(buffer, 0, maxBlockSize);

                        while (blockSize > 0)
                        {
                            var blockByte = new byte[blockSize];

                            Array.Copy(buffer, 0, blockByte, 0, blockSize);

                            var decrypts = rsa.Decrypt(blockByte, padding);

                            readStream.Write(decrypts, 0, decrypts.Length);

                            blockSize = memoryStream.Read(buffer, 0, maxBlockSize);
                        }

                        return readStream.ToArray();
                    }
                }
            }
        }

        public void FromXmlString(RSA rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        int GetMaxBlockSize(RSA rsa, RSAEncryptionPadding padding)
        {
            var offset = 0;
            if (padding.Mode == RSAEncryptionPaddingMode.Pkcs1)
            {
                offset = 11;
            }
            else
            {
                if (padding.Equals(RSAEncryptionPadding.OaepSHA1))
                {
                    offset = 42;
                }

                if (padding.Equals(RSAEncryptionPadding.OaepSHA256))
                {
                    offset = 66;
                }

                if (padding.Equals(RSAEncryptionPadding.OaepSHA384))
                {
                    offset = 98;
                }

                if (padding.Equals(RSAEncryptionPadding.OaepSHA512))
                {
                    offset = 130;
                }
            }
            return rsa.KeySize / 8 - offset;
        }
        static RSA rsaKey = RSA.Create();
        public static string CreatePrivateKey()
        {
            return rsaKey.ToXmlString2(true);
        }
        public static string CreatePublicKey()
        {
            return rsaKey.ToXmlString2(false);
        }
    }

    public static class RsaExtention
    {

        public static void FromXmlString2(this RSA rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                        case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
                        case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
                        case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
                        case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
                        case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        public static string ToXmlString2(this RSA rsa, bool includePrivateParameters)
        {
            RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

            if (includePrivateParameters)
            {
                return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                    Convert.ToBase64String(parameters.Modulus),
                    Convert.ToBase64String(parameters.Exponent),
                    Convert.ToBase64String(parameters.P),
                    Convert.ToBase64String(parameters.Q),
                    Convert.ToBase64String(parameters.DP),
                    Convert.ToBase64String(parameters.DQ),
                    Convert.ToBase64String(parameters.InverseQ),
                    Convert.ToBase64String(parameters.D));
            }
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                    Convert.ToBase64String(parameters.Modulus),
                    Convert.ToBase64String(parameters.Exponent));
        }

    }
   
}

