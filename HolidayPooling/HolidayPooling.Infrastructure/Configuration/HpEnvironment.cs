using HolidayPooling.Infrastructure.Configuration;
using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Configuration;
using Sams.Commons.Infrastructure.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: ConfiguratorAttribute(ConfigurationType.Database, typeof(DatabaseConfigurator), "DbSettings.xml")]


namespace HolidayPooling.Infrastructure.Configuration
{
    public class HpEnvironment : AppEnvironmentSetup, IDisposable
    {

        #region Fields

        private bool _disposed;

        #endregion

        #region AppEnvironmentSetup

        private new HpEnvironment SetCurrentEnvironment(AppEnvironment env)
        {
            _currentEnv = env;
            return this;
        }

        private HpEnvironment ConstructBudgetAppBasePath()
        {
            base.ConstructBasePath();
            return this;
        }

        public override void SetupEnvironment(AppEnvironment env)
        {
            Check.That(env, (e) => e != AppEnvironment.None, "env");
            SetCurrentEnvironment(env)
                .ConstructBudgetAppBasePath()
                .ConfigureDatabases();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                ConnectionManager.Clear();
                this._basePath = null;
                this._currentEnv = AppEnvironment.None;
            }

            _disposed = true;
        }

        ~HpEnvironment()
        {
            Dispose(false);
        }

        #endregion

    }
}