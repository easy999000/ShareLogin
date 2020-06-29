using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace CoreWeb.code.HQShareLogin
{
    public class HQSecureDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        string PrivateKey = "<RSAKeyValue><Modulus>yt/tIiQzqJd7EUWlCOncHLU100K073OhIyDNCzF93Jvp6D9cAYLrwcqcofOnLcVPl9Ixfe2XF85cuWRsCHXkwFALpwZrGitNGXRvGh2jJLJRNpxy+lEC91uvNIS6/2nOXAsEQqBmnD0+txdeq+R6XaOSr8gM0O0toupGH2wg7LRNTxKHudyvX04YHYpJW2ys1GgfelFyS6RctJWNMlriuYjZvhuJMzgfS2Lji5tCjBlx6k7ToqHVtDhmDcx/H6XKb4oBLlKilnz26s7RSY5oKj5BL2qrVPaMgejQjA2oFlLLKNbapipydoynyJ8Rw/nzq/svDY2tfdwGHF+C9dEnyQ==</Modulus><Exponent>AQAB</Exponent><P>1BVOgjnEc2e+3veNwcCc0I98HPpvbHPW/A94YueS+GNb/2JPPzgJVqcI0efZP0bsmvj4dOF3WpzhkFSm4G6ZTwRmbe0NGZJCjr5sEMJVAQZb6ODkozs4iA1TAtOcgQ+mASrY8SbY48sqXBU1TOq35C746R2KmYoZIwzkOTEqS6c=</P><Q>9OJ4GQP6av/duz2oep0lh9ovYWBkKAIbmbMApK6BXx7+u3DgNZvNY1PMg9cTUvm8yKAk64QTeontHryr6A8ZzCknAWA4nxABTjIUUq9BVidYvomS1gSE/sPxHPtaEcNagZhlFE9VDrFeNYFg20WcAfFoVwvEt8pXPCRO1hkWnw8=</Q><DP>IgYxDC9cVu9j7b1lXpoaDlOWo8eBrLKA4PtSYvun1nWKQtRwxkGlLeOqRJKfhclJGCutIIMzdLQKc+9Avmy/569bB3OqUFnol6HxEFc87+cnQ5sg0xcjIKCSmrd8OjBaf4FyQG3tCc6EQzWb0XUuf0sR38Q/ghZnpfnhe1Np560=</DP><DQ>1T5EkJwLO5O1lPA7PxKK9QMFEUmrzb8QI8ZhgAZJt/g4oCbJx0T2FFGLnR7zv+SjBR2XKNNpVuK2bnajR7+mmkcXpTYR+EkqqdooIlxki85VlA4epGlpGqSr2K30H9W6gXGgS5BNpC7Ft9gC7M5v7K04S9x+oAD99+wyDnPoY/c=</DQ><InverseQ>c3fv2/TLjDRiWEUTArGoWSJoim4j0ZsqER6WsRKF+e4+fSGEFRiydTZvFxd8kqHN9YxHPEImMgPyWO+4ssVH8jPSbd2n7fNNvylUjpPHV9/H/wBvIqrskXqG597omSU8dQp1AcA5vjzrQhpBg2E7C4XYwupGdC5KYXLVEFotWeQ=</InverseQ><D>ZxnqD4HKZxGkz6BPQPpojIZjNlweS9q9t3aB/gBG8IikFQ2uGm6IldH8TCnheldeHdAKqu7F7KlJYkZwyNPxTvLfGSEf6Qq8ggU0OVd6g7nDoDdeD2yAT2le3xfdWNRStWA76AhQXXKanr3XtQ/GSDRK5d6K9Nq0aKkavflbui73Js5E/sbAXkDrtdiRSZD4mc4HLFPxGxrf+kcYd9U6Bu7/c3BK4jZDIO7vc2gL3Cq0e/rfj9pSw4tIHYNVl3QYMTbEhFTlyOIEG7skA6/kIhNj/D2D5Fk4VQ89aruCaNY2SLulDGAMQek1xnPVEN5A4AN47rHqcHpvN/E9T6FcCQ==</D></RSAKeyValue>";
        string PublicKey = "<RSAKeyValue><Modulus>yt/tIiQzqJd7EUWlCOncHLU100K073OhIyDNCzF93Jvp6D9cAYLrwcqcofOnLcVPl9Ixfe2XF85cuWRsCHXkwFALpwZrGitNGXRvGh2jJLJRNpxy+lEC91uvNIS6/2nOXAsEQqBmnD0+txdeq+R6XaOSr8gM0O0toupGH2wg7LRNTxKHudyvX04YHYpJW2ys1GgfelFyS6RctJWNMlriuYjZvhuJMzgfS2Lji5tCjBlx6k7ToqHVtDhmDcx/H6XKb4oBLlKilnz26s7RSY5oKj5BL2qrVPaMgejQjA2oFlLLKNbapipydoynyJ8Rw/nzq/svDY2tfdwGHF+C9dEnyQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        RSAHelper rsa;
        private readonly IDataSerializer<AuthenticationTicket> _serializer;

        public HQSecureDataFormat(string privateKey = null, string publicKey = null)
        {
            if (!string.IsNullOrWhiteSpace(privateKey))
            {
                PrivateKey = privateKey;
            }
            if (!string.IsNullOrWhiteSpace(publicKey))
            {
                PublicKey = publicKey;
            }
            rsa = new RSAHelper(PrivateKey, PublicKey);
            _serializer = TicketSerializer.Default;
        }

        public string Protect(AuthenticationTicket data)
        {
            var userData = _serializer.Serialize(data);


            var protectedData = rsa.Encrypt(userData);
            var s1 = Base64UrlTextEncoder.Encode(protectedData);
            return s1;
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            return Protect(data);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            try
            {
                if (protectedText == null)
                {
                    return default(AuthenticationTicket);
                }

                var protectedData = Base64UrlTextEncoder.Decode(protectedText);
                if (protectedData == null)
                {
                    return default(AuthenticationTicket);
                }


                var userData = rsa.Decrypt(protectedData);
                if (userData == null)
                {
                    return default(AuthenticationTicket);
                }
                var v1= _serializer.Deserialize(userData);
                var v2 = v1.Properties.ExpiresUtc;
                return v1;
            }
            catch
            {
                // TODO trace exception, but do not leak other information
                return default(AuthenticationTicket);
            }
        }

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            return Unprotect(protectedText);
        }
    }
}
