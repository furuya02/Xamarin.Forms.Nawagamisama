using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;


namespace nawagamisama {
    public class App {
        public static Page GetMainPage(){
            return new MyPage();
        }
    }

    class MyPage : ContentPage{

        //オブジェクト配列
        static List<Hole> _holes;
        private int HoleMax = 9;
        private int ImageMax = 10;
        private Button _button;
        private bool _play = false;
        private DateTime _time;
        public MyPage() {
            
            //画像のソース（全ホール用を、ここで確保して共有する）
            var imageSources = new List<ImageSource>();
            for (var i = 0; i < ImageMax; i++){
                imageSources.Add(ImageSource.FromResource(string.Format("nawagamisama.Images.image{0}.jpg", i)));
            }
            //ホールオブジェクトの生成
            _holes = new List<Hole>();
            for (var i = 0; i < HoleMax; i++) {
                //オブジェクトの生成
                _holes.Add(new Hole(imageSources));
            }

            _button = new Button{
                Text = "Start",
                BackgroundColor = Color.Blue
            };
            _button.Clicked += (sender, args) => {
                if (!_play){
                    foreach (var hole in _holes){
                        hole.Init();
                    }
                    _play = true;
                    _time = DateTime.Now.AddSeconds(30);
                }

            };

            var layout = new StackLayout();
            layout.Children.Add(_button);//DEBUG

            //縦に（横３つの穴）を並べる
            for (var y = 0; y < 3; y++) {
                //横に３つの穴を並べる
                var l = new StackLayout { Orientation = StackOrientation.Horizontal };
                for (var x = 0; x < 3; x++) {
                    l.Children.Add(_holes[y * 3 + x]);
                }
                layout.Children.Add(l);
            }

            Content = layout;

            BackgroundColor = Color.White;
            var padding = 5;
            Padding = padding;

            //サイズが決定したタイミングでHoleのサイズが決定される
            SizeChanged += (sender, args) =>{
                var size = (Width > Height)?Height/3:Width/3;
                size -= padding * 2;
                foreach (var hole in _holes) {
                        hole.Size = (int)size;
                }
            };

            //タイマ処理
            Device.StartTimer(new TimeSpan(10000 * 500), () =>{
                if (_play){
                    var span = _time - DateTime.Now;
                    var score = 0;
                    foreach (var hole in _holes) {
                        Task.Run(() => Device.BeginInvokeOnMainThread(() => hole.Animation()));
                        score += hole.Score;
                    }
                    if (span.Ticks < 0) {
                        //GAME OVER
                        _play = false;
                        _button.Text = string.Format("GACE OVER  SCORE:{0}  [restart]", score);
                        return true;
                    }

                    _button.Text = string.Format("{0:D2}:{1:D2} SCORE:{2}", span.Minutes,span.Seconds,score);
                }
                return true;
            });


        }
    }

    

    
}
