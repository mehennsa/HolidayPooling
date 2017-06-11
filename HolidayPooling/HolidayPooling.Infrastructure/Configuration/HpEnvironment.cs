using HolidayPooling.Infrastructure.Configuration;
using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Configuration;
using Sams.Commons.Infrastructure.Environment;
using System;

[assembly: ConfiguratorAttribute(ConfigurationType.Database, typeof(DatabaseConfigurator), "DbSettings.xml")]
[assembly: ConfiguratorAttribute(ConfigurationType.Log, typeof(LogConfiguration), "log.config")]

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
                .ConfigureLog()
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

        protected HpEnvironment ConfigureLog()
        {
            base.ConfigureLog();
            return this;
        }

        ~HpEnvironment()
        {
            Dispose(false);
        }

        #endregion

    }
}