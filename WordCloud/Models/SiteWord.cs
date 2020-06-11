using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WordCloud.Models
{
    public class SiteWord
    {
        [Key]
        public string Id { get; set; }

        private byte[] salt = System.Text.Encoding.Unicode.GetBytes("H1r3Me1Mc00l");

        [Required]
        public string Word { get; set; }

        [Required]
        public int Count { get; set; }

        public SiteWord(string word, int count = 1)
        {
            Id = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: word,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
            Word = word;
            Count = count;
        }
    }
}
