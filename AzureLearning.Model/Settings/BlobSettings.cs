using System;
using System.Collections.Generic;
using System.Text;

namespace AzureLearning.Model.Settings
{
    public class BlobSettings
    {
        public string? VaultUri { get; set; }
        public string? BlobConnectionStringSecretName { get; set; }
        public string? BlobContainerNameSecretName { get; set; }
    }
}
