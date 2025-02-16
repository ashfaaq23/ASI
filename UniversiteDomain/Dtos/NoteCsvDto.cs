using CsvHelper.Configuration.Attributes;

namespace UniversiteDomain.Dtos
{
    public class NoteCsvDto
    {
        [Name("NumEtud")] // CSV Column Name
        public long NumEtud { get; set; }

        [Name("Nom")]
        public string Nom { get; set; } = string.Empty;

        [Name("Prenom")]
        public string Prenom { get; set; } = string.Empty;

        [Name("NumeroUe")]
        public long NumeroUe { get; set; }

        [Name("Intitule")]
        public string Intitule { get; set; } = string.Empty;

        [Name("Note")]
        public float? Note { get; set; } // Nullable, as notes can be empty in the CSV
    }
}