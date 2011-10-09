using System;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;


namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public class DiagnosticEntry
    {
#if IPHONE
		private static System.Resources.ResourceManager _Resources;
#else
        private static System.ComponentModel.ComponentResourceManager _Resources;
#endif

#if !IPHONE
        private System.ComponentModel.ComponentResourceManager _MyResource
#else
		private System.Resources.ResourceManager _MyResource
#endif
		{
            get
            {
                if (_Resources == null)
#if !IPHONE
                    _Resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosticEntry));
#else
					_Resources = new System.Resources.ResourceManager(typeof(DiagnosticEntry));			
#endif
				return _Resources;
            }
        }

        private DateTime _TimeStamp;
        public DateTime TimeStamp { get { return _TimeStamp; } }

        public string Message { get { return _MyResource.GetString("ID_0x" + _id.ToString("X")); } }

        private int _id;
        public int ID { get { return _id; } }

        private int _ob;
        
        private byte[] _extInfo;

        
        public DiagnosticEntry(byte[] data)
        {
            //_Resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosticEntry));
            _TimeStamp = libnodave.getDateTimefrom(data, 12);
            _id = data[0] * 256 + data[1]; 
            //Bytes 2-11 additional Info!        
        }

        /*
        public DiagnosticEntry(int id)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosticEntry));

            _id = id;
            //Message = resources.GetString("ID_0x" + _id.ToString("X"));

            switch (_id)
            {
                case (0x2521):
                case (0x2522):
                case (0x2523):
                case (0x2524):
                case (0x2525):
                case (0x2526):
                case (0x2527):
                case (0x2528):
                case (0x2529):
                case (0x2530):
                case (0x2531):
                case (0x2532):
                case (0x2533):
                case (0x2534):
                case (0x2535):
                case (0x253A):
                case (0x253C):
                case (0x253D):
                case (0x253E):
                case (0x253F):
                case (0x2942):
                case (0x2943):
                    _ob = 121;
                    break;
                case (0x3501):
                case (0x3502):
                case (0x3503):
                case (0x3505):
                case (0x3506):
                case (0x3507):
                case (0x3508):
                case (0x3509):
                case (0x350A):
                case (0x350B):
                    _ob = 80;
                    break;
                case (0x3921):
                case (0x3821):
                case (0x3922):
                case (0x3822):
                case (0x3923):
                case (0x3823):
                case (0x3925):
                case (0x3825):
                case (0x3926):
                case (0x3826):
                case (0x3927):
                case (0x3827):
                case (0x3931):
                case (0x3831):
                case (0x3932):
                case (0x3832):
                case (0x3933):
                case (0x3833):
                    _ob = 81;
                    break;
                case (0x3942):
                    _ob = 82;
                    break;
                case (0x3842):
                    _ob = 82;
                    break;
                case (0x3951):
                case (0x3954):
                case (0x3854):
                case (0x3855):
                case (0x3856):
                case (0x3858):
                case (0x3861):
                case (0x3961):
                case (0x3863):
                case (0x3864):
                case (0x3865):
                case (0x3866):
                case (0x3966):
                case (0x3367):
                case (0x3267):
                case (0x3968):
                    _ob = 83;
                    break;
                case (0x3571):
                case (0x3572):
                case (0x3573):
                case (0x3574):
                case (0x3575):
                case (0x3576):
                case (0x3578):
                case (0x357A):
                    _ob = 88;
                    break;
                case (0x3884):
                case (0x3984):
                    _ob = 83;
                    break;
                case (0x3981):
                case (0x3881):
                case (0x3582):
                case (0x3583):
                case (0x3585):
                case (0x3986):
                case (0x3587):
                    _ob = 84;
                    break;
                case (0x35A1):
                case (0x35A2):
                case (0x35A3):
                case (0x35A4):
                case (0x34A4):
                case (0x39B1):
                case (0x39B2):
                case (0x39B3):
                case (0x38B3):
                case (0x39B4):
                case (0x38B4):
                    _ob = 85;
                    break;
                case (0x38C1):
                case (0x39C1):
                case (0x38C2):
                case (0x39C3):
                case (0x39C4):
                case (0x38C4):
                case (0x39C5):
                case (0x38C5):
                case (0x38C6):
                case (0x38C7):
                case (0x38C8):
                case (0x39CA):
                case (0x39CB):
                case (0x38CB):
                case (0x39CC):
                case (0x38CC):
                case (0x39CD):
                case (0x39CE):
                    _ob = 86;
                    break;
                case (0x35D2):
                case (0x35D3):
                case (0x35D4):
                case (0x35D5):
                case (0x35E1):
                case (0x35E2):
                case (0x35E3):
                case (0x35E4):
                case (0x35E5):
                case (0x35E6):
                    _ob = 87;
                    break;



                #region VIPA Additional Errorcodes
                case (0xE003):
                case (0xE004):
                case (0xE005):
                case (0xE006):
                case (0xE007):
                case (0xE008):
                case (0xE009):
                case (0xE010):
                case (0xE011):
                case (0xE012):
                case (0xE013):
                case (0xE014):
                case (0xE015):
                case (0xE016):
                case (0xE017):
                case (0xE018):
                case (0xE019):
                case (0xE01A):
                case (0xE01B):
                case (0xE030):
                case (0xE0B0):
                case (0xE0C0):
                case (0xE0CC):
                case (0xE0CD):
                case (0xE0CE):
                case (0xE100):
                case (0xE101):
                case (0xE102):
                case (0xE104):
                case (0xE200):
                case (0xE210):
                case (0xE21F):
                case (0xE400):
                case (0xE401):
                case (0xE801):
                case (0xE802):
                case (0xE803):
                case (0xE804):
                case (0xE805):
                case (0xE806):
                case (0xE807):
                case (0xE80B):
                case (0xE80E):
                case (0xE8FB):
                case (0xE8FC):
                case (0xE8FE):
                case (0xE8FF):
                case (0xE901):
                case (0xEA00):
                case (0xEA01):
                case (0xEA02):
                case (0xEA04):
                case (0xEA05):
                case (0xEA07):
                case (0xEA08):
                case (0xEA09):
                case (0xEA10):
                case (0xEA11):
                case (0xEA12):
                case (0xEA14):
                case (0xEA15):
                case (0xEA18):
                case (0xEA19):
                case (0xEA20):
                case (0xEA21):
                case (0xEA22):
                case (0xEA23):
                case (0xEA24):
                case (0xEA30):
                case (0xEA40):
                case (0xEA41):
                case (0xEA98):
                case (0xEA99):
                case (0xEE00):
                    break;
                #endregion

            }         
        }*/

        public override string ToString()
        {
            return "ID:" + _id.ToString() + " " + Message;
        }
    }
}
