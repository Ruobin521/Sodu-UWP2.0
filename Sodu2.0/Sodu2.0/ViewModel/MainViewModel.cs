﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Sodu.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<string> _testList;

        public ObservableCollection<string> TestCollection
        {
            get
            {
                if (_testList == null)
                {
                    _testList = new ObservableCollection<string>()
                    {
                        "M1298.724571 1013.906286 192.073143 1013.906286l2.048-27.794286c13.312-167.643429 185.197714-308.516571 419.693714-346.550857l0-62.756571c-76.068571-55.588571-122.441143-154.331429-122.441143-262.290286 0-168.667429 114.102857-305.444571 253.074286-305.444571 139.849143 0 253.074286 136.777143 253.074286 305.444571 0 107.958857-46.226286 206.701714-122.441143 262.290286l0 62.756571C1109.430857 678.619429 1281.170286 818.468571 1294.628571 986.112L1298.724571 1013.906286zM249.709714 962.56l991.524571 0c-28.818286-135.753143-185.197714-247.808-393.947429-275.602286l-22.674286-3.072 0-134.729143 11.264-7.168c68.900571-44.178286 111.030857-130.633143 111.030857-227.328 0-140.873143-90.550857-255.122286-202.605714-255.122286-111.030857 0-202.605714 114.102857-202.605714 255.122286 0 95.670857 42.130286 183.003429 111.030857 227.328l11.264 7.168 0 134.729143-22.674286 3.072C435.785143 714.605714 278.381714 826.806857 249.709714 962.56z"
                    };
                }
                return _testList;
            }
            set { Set(ref _testList, value); }
        }



    }
}
