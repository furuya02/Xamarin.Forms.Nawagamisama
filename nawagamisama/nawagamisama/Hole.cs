using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace nawagamisama {
    //ホール（穴）オブジェクト
    class Hole : ContentView {
        private readonly List<ImageSource> _imageSources;
        private readonly Image _image;
        private readonly Button _button;

        private int _size;
        public int Size {
            set {
                _size = value;
                OnPropertyChanged("Size");
            }
        }

        public int Score { get; set; }

        public Hole(List<ImageSource> imageSources){

            //画像ソースのリスト
            _imageSources = imageSources;

            //************************************************
            //画像の配置
            //************************************************
            var absoluteLayout = new AbsoluteLayout();
            _image = new Image {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Source = _imageSources[0],
            };
            //************************************************
            //タップのイベントを拾うため透過のボタンを配置する
            //************************************************
            absoluteLayout.Children.Add(_image, new Point(0, 0));
            _button = new Button {
                BackgroundColor = Color.Transparent,//透過
            };
            if (Device.OS != TargetPlatform.iOS) {
                _button.Opacity = 0;//iOSでは、イベントが拾えなくなる
            }
            _button.Clicked += button_Clicked;//タップ時のイベント処理
            absoluteLayout.Children.Add(_button, new Point(0, 0));


            Content = absoluteLayout;

            Init();
        }

        private readonly Random _random = new Random((int)DateTime.Now.Ticks);
        private Status _status = Status.Sleep;
        private int _count;
        private int _pattern;

        //タイマーイベント
        public void Animation() {
            _count--;
            if (_count > 0) {
                return;
            }
            if (_status == Status.Sleep){
                _status = Status.Try;
                _count = 3; //出現時間

                if (0 == _random.Next(5)){
                    //パターン１～４は、まれに出現
                    _pattern = _random.Next(4) + 1;
                } else{
                    //パターン5は、通常出現
                    _pattern = 5;
                }
                _image.Source = _imageSources[_pattern];

            } else if (_status == Status.Die || _status == Status.Try) {

                if (_status == Status.Try){
                    //死ななかった場合
                    Score -= 50; //縄神様の勝ち
                }

                _status = Status.Sleep;
                _count = _random.Next(10);
                _image.Source = _imageSources[0];
            }
        }

        //タップ時のイベント処理
        private void button_Clicked(object sender, EventArgs e) {
            if (_status == Status.Try){
                //死んだ時のパターン番号
                var dieNo = 6;//パターン1～４の場合
                if (_pattern == 5) { //　パターン5の場合は ７，８，９のどれか
                    dieNo = _random.Next(3) + 7;
                }
                switch (dieNo){
                    case 6:
                        Score += 100;
                        break;
                    case 7:
                        Score += 10;
                        break;
                    case 8:
                        Score += 10;
                        break;
                    case 9:
                        Score += 20;
                        break;
                }
                _image.Source = _imageSources[dieNo];
                _count = 3;
                _status = Status.Die;

            }
        }

        protected override void OnPropertyChanged(string propertyName = null) {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "Size") {
                _button.WidthRequest = _size;
                _button.HeightRequest = _size;
                _image.WidthRequest = _size;
                _image.HeightRequest = _size;
            }
        }
        //再起動
        internal void Init(){
            Score = 0;
            _status = Status.Sleep;
            _count = _random.Next(10);
            _image.Source = _imageSources[0];
        }
    }
}
