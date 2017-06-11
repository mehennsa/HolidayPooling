using HolidayPooling.Infrastructure.Converters;
using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Configuration;
using Sams.Commons.Infrastructure.Crypters;
using Sams.Commons.Infrastructure.Xml;
using System.Text;
using System.Xml;

namespace HolidayPooling.Infrastructure.Configuration
{
    public sealed class DatabaseConfigurator : BaseXmlConfigurator
    {

        #region Fields

        private const string ConnectionStringSeparator = ";";
        private const string Equal = "=";
        private const string User = "User Id";
        private const string Password = "Password";
        private const string SettingsNode = "/Settings/Setting";
        private const string ConnectionStringAttribute = "connectionString";
        private const string LoginAttribute = "login";
        private const string PasswordAttribute = "password";
        private const string NameAttribute = "name";

        #endregion

        #region Methods

        private string ConstructFullConnectionString(string connectionString, string login, string password)
        {
            Check.IsNotNullOrEmpty(connectionString, "connectionString");
            Check.IsNotNullOrEmpty(login, "login");
            Check.IsNotNullOrEmpty(password, "password");
            var builder = new StringBuilder();
            builder.Append(connectionString)
                   .Append(ConnectionStringSeparator)
                   .Append(User)
                   .Append(Equal)
                   .Append(StringCrypter.Decrypt(login))
                   .Append(ConnectionStringSeparator)
                   .Append(Password)
                   .Append(Equal)
                   .Append(StringCrypter.Decrypt(password));
            return builder.ToString();
        }

        #endregion

        #region IConfigurator

        protected override void ParserHandler(IXmlParser parser)
        {
            foreach (XmlNode node in parser.GetNodeList(SettingsNode))
            {
                var conString = parser.GetNodeAttributeValue(ConnectionStringAttribute, node);
                var login = parser.GetNodeAttributeValue(LoginAttribute, node);
                var password = parser.GetNodeAttributeValue(PasswordAttribute, node);
                string fullConnectionString = ConstructFullConnectionString(conString, login, password);
                HolidayPoolingDatabase db = TechnicalEnumConverter.
                    HolidayPoolingDatabaseFromString(parser.GetNodeAttributeValue(NameAttribute, node));
                ConnectionManager.AddConnection(db, fullConnectionString);
            }
        }

        #endregion
    }
}