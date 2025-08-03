using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Shared.Constants
{
    public class SystemConstants
    {
        public const string MainConnectionString = "BaseSourceDbConnection";
        public const string SiteAuthorName = "CryptoHub";

        public class AppSettings
        {
            public const string DefaultLanguageId = "DefaultLanguageId";
            public const string Token = "Token";
            public const string BackendApiClient = "BackendApiClient";
            public const string UploadApiClient = "UploadApiClient";
        }
        public class TwoFA
        {
            public const string PrivateKey2Fa = "WjdA7YU2y6vC";
            public const string Issuer = "CryptoHub";
        }

        public static class LangId
        {
            public const string en = "en";
            public const string vi = "vi";
        }
        public const string LanguageDefault = "vi";
        public static readonly ReadOnlyCollection<String> Languages =
            new ReadOnlyCollection<string>(new List<String> { LangId.vi, LangId.en });

#if DEBUG
        public const bool IS_DEBUG = true;
#else
        public const bool IS_DEBUG = false;
#endif
    }

    public class SQL_CONST
    {
        public const string Collation = "SQL_Latin1_General_CP1_CI_AI";
    }
}
