using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    class Settings
    {
        #region PHPServerURL
        private string phpServerUrl;
        public string PHPServerURL
        {
            get
            {
                if (phpServerUrl == null || phpServerUrl == string.Empty)
                {
                    getSettings();
                }
                return phpServerUrl;
            }
        }
        #endregion
        #region CopyCount
        private string copyCount;
        public string CopyCount
        {
            get
            {
                if (CopyCount == null || CopyCount == string.Empty)
                {
                    getSettings();
                }
                return CopyCount;
            }
        }
        #endregion
        #region PhotoURL
        private string photoURL;
        public string PhotoURL
        {
            get
            {
                if (photoURL == null || photoURL == string.Empty)
                {
                    getSettings();
                }
                return photoURL;
            }
        }
        #endregion
        #region PHPServerUsername
        private string phpServerUsername;
        public string PHPServerUsername
        {
            get
            {
                if (phpServerUsername == null)
                {
                    getSettings();
                }
                return phpServerUsername;
            }
        }
        #endregion
        #region PHPServerPassword
        private string phpServerPwd;
        public string PHPServerPassword
        {
            get
            {
                if (phpServerPwd == null)
                {
                    getSettings();
                }
                return phpServerPwd;
            }
        }
        #endregion
        #region COST
        private double _cost;
        public double COST
        {
            get
            {
                if (_cost == null ||_cost == 0)
                {
                    getSettings();
                }
                return _cost;
            }
        }
        #endregion
        #region Instance
        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                if (instance == null) instance = new Settings();
                return instance;
            }
        }
        #endregion
        private string settingsFileName;
        Settings()
        {
            this.settingsFileName = "settings.usr";
        }
        private void getSettings()
        {
            if (!File.Exists(settingsFileName)) return;
            foreach (string line in File.ReadAllLines(settingsFileName))
            {
                string[] code = line.Split('#');
                if (code.Length != 2) continue;
                switch (code[1].Trim())
                {
                    case "URL":
                        phpServerUrl = code[0].Trim();
                        break;
                    case "KULLANICIADI":
                        phpServerUsername = code[0].Trim();
                        break;
                    case "SIFRE":
                        phpServerPwd = code[0].Trim();
                        break;
                    case "FOTOGRAFURL":
                        photoURL = code[0].Trim();
                        break;
                    case "KOPYASAYISI":
                        photoURL = code[0].Trim();
                        break;
                    case "FIYAT":
                        try
                        {
                            _cost = Double.Parse(code[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                            _cost = 1;
                        }
                        break;
                }
            }
        }
        private void setAll(ArrayList list)
        {
            phpServerUrl = list[0].ToString();
            phpServerUsername = list[1].ToString();
            phpServerPwd = list[2].ToString();
        }
       
    }
}
