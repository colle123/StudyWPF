using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPFNaverMovieFinder.Models;

namespace WPFNaverMovieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class MainWindow : MetroWindow
    {
        bool IsFavorite = false; // 네이버 API로 검색한건지, 즐겨찾기DB에서 온것인지 확인할 값
        // IsFavorite == True -> DB에서 온값 / IsFavorite == false -> 네이버 API

        public MainWindow()
        {
            InitializeComponent();
        }
        private void txtSearchName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //검색에서 엔터를 KeyDown(키를 누르면) btnSearch_Click이 실행됨.
            if (e.Key == System.Windows.Input.Key.Enter) btnSearch_Click(sender, e);
        }

        ///<summary>
        /// 검색버튼 클릭 이벤트 핸들러
        /// 네이버 OpenAPI 검색
        /// </summary>
        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //검색을 시작할 때 stsResult의 값을 공백으로 만들기 위해서 string.Empty를 사용
            stsResult.Content = string.Empty;

            //string.IsNullOrEmpty() = 텍스트 안이 공백이나 null 값이면 1을 반환
            if (string.IsNullOrEmpty(txtSearchName.Text))
            {
                stsResult.Content = "검색할 영화명을 입력, 검색버튼을 눌러주세요.";
                //MessageBox.Show("검색할 영화명을 입력, 검색버튼을 눌러주세요.");
                Commons.ShowMessageAsync("검색", "검색할 영화명을 입력, 검색버튼을 눌러주세요.");

                return;
            }

            // 검색시작
            // Commons.ShowMessageAsync("결과", $"{txtSearchName.Text}");

            try
            {
                SearchNaverOpenAPI(txtSearchName.Text);
                Commons.ShowMessageAsync("검색", "영화검색 완료!!");
                IsFavorite = false; // API로 검색했으므로
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
            }

        }

        private void SearchNaverOpenAPI(string searchName)
        {
            string clientID = "_Qjhnv3RpqvZSN06Qd3B";
            string clientSecret = "WzLF7rg4NU";
            string openApiUri = $"https://openapi.naver.com/v1/search/movie?start=1&display=30&query={searchName}";
            string result = string.Empty; // 빈값 초기화

            WebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;

            // Naver OpenAPI 실제 요청
            try
            {
                request = WebRequest.Create(openApiUri);
                request.Headers.Add("X-Naver-Client-Id", clientID); // 중요!
                request.Headers.Add("X-Naver-Client-Secret", clientSecret); // 중요!!

                response = request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream);

                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {               
                reader.Close();
                stream.Close();
                response.Close();
            }         

            var parseJson = JObject.Parse(result);

            int total = Convert.ToInt32(parseJson["total"]); // 전체 검색결과 수
            int display = Convert.ToInt32(parseJson["display"]);

            stsResult.Content = $"{total}개 중 {display}개 호출 성공!";

            // 데이터그리드에 검색결과 할당
            var items = parseJson["items"];
            var json_array = (JArray)items;

            List<MovieItem> movieItems = new List<MovieItem>();

            foreach (var item in json_array)
            {
                MovieItem movie = new MovieItem(
                        //해당하는 값((.|\n)*?을 전부 공백으로 만들어(정규화)해버림
                        Regex.Replace(item["title"].ToString(), @"<(.|\n)*?>", string.Empty),
                        item["link"].ToString(),
                        item["image"].ToString(),
                        item["subtitle"].ToString(),
                        item["pubDate"].ToString(),
                        // |를 ,로 바꿔줌
                        item["director"].ToString().Replace("|", ", "),
                        item["actor"].ToString().Replace("|", ", "),
                        item["userRating"].ToString());
                movieItems.Add(movie);
            }

            this.DataContext = movieItems;
        }

        private void btnAddWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(grdResult.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택하세요(복수선택 가능.");
                return;
            }

            if(IsFavorite == true)
            {
                Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 영화입니다.");
                return;
            }

            List<TblFavoriteMovies> list = new List<TblFavoriteMovies>();
            foreach (MovieItem item in grdResult.SelectedItems)
            {
                TblFavoriteMovies temp = new TblFavoriteMovies()
                {
                    Title = item.Title,
                    Link = item.Link,
                    Image = item.Image,
                    SubTitle = item.SubTitle,
                    PubDate = item.PubDate,
                    Director = item.Director,
                    Actor = item.Actor,
                    UserRating = item.UserRating,
                    RegDate = DateTime.Now
                };

                list.Add(temp);
            }

            //EF 테이블 데이터 입력(INSERT)
            try
            {
                using(var ctx = new OpenAplLabEntities())
                {
                    foreach (var item in list)
                    {
                        ctx.Set<TblFavoriteMovies>().Add(item);
                    }
                    ctx.SaveChanges(); // Commit
                }

                Commons.ShowMessageAsync("저장", "즐겨찾기 추가 성공!!");
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");              
            }
        }

        private void btnViewWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DataContext = null;
            txtSearchName.Text = string.Empty;

            List<TblFavoriteMovies> list = new List<TblFavoriteMovies>();
            try
            {
                using (var ctx = new OpenAplLabEntities())
                {
                    list = ctx.TblFavoriteMovies.ToList();
                }

                this.DataContext = list;
                stsResult.Content = $"즐겨찾기 {list.Count}개 조회";
                Commons.ShowMessageAsync("즐겨찾기", "즐겨찾기 조회 완료!");
                IsFavorite = true; // DB에서 가져왔으니 true로 변경
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                IsFavorite = false;
            }
        }

        private void btnDelWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(IsFavorite == false)
            {
                Commons.ShowMessageAsync("오류", "즐겨찾기 내용이 아니면 삭제할 수 없습니다.");
                return;
            }

            if(grdResult.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("오류", "삭제할 영화를 선택하세요.");
                return;
            }

            foreach (TblFavoriteMovies item in grdResult.SelectedItems)
            {
                using (var ctx = new OpenAplLabEntities())
                {
                    //삭제처리
                    var delItem = ctx.TblFavoriteMovies.Find(item.Idx); // PK
                    ctx.Entry(delItem).State = System.Data.EntityState.Deleted; // 검색해서 나온 객체를 삭제상태로 변경
                    ctx.SaveChanges(); // Commit처리
                }
            }

            btnViewWatchList_Click(sender, e); // 삭제처리 이후 즐겨찾기보기 버튼클릭 이벤트 실행
        }
        /// <summary>
        /// 유튜브 예고편 보기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWatchTrailer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (grdResult.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("유튜브영화", "영화를 선택하세요.");
                return;
            }

            if (grdResult.SelectedItems.Count > 1)
            {
                Commons.ShowMessageAsync("유튜브영화", "영화를 하나만 선택하세요.");
                return;
            }

            string movieName = ""; // string.Empty;

            if (IsFavorite == true) // 즐겨찾기 DB값이면 if문 내용을 처리, 아니면(네이버 API 값이면) else문을 처리
            {
                movieName = (grdResult.SelectedItem as TblFavoriteMovies).Title;
            }
            else
            {
                movieName = (grdResult.SelectedItem as MovieItem).Title; // 한글 영화제목을 가져옴
            }

            var trailerWindow = new TrailerWindow(movieName); // 생성자를 통해서 movieName을 TrailerWindow로 넘겨줌.
            trailerWindow.Owner = this; // 여기서 this는 MainWindow
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // MainWindow 정중앙에 띄운다는 뜻
            trailerWindow.ShowDialog();
            // Show를 쓰면 MainWindow와 TrailerWindow가 같이 뜨고 클릭이 됨. = modal 창이라는 뜻
            // 예고편이 종료되기 전까지 메인창을 건들이면 안되니 modaless로 만들어야 하기 때문에 ShowDialog로 만들어야 함.



        }

        private void grdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if(grdResult.SelectedItem is MovieItem) // 네이버 API에서 온 값이면
            {
                var movie = grdResult.SelectedItem as MovieItem;

                if (string.IsNullOrEmpty(movie.Image))
                {
                    //movie.Image에 값이 없으면 Resource에 저장된 No_Picture를 불러옴.
                    imgPoster.Source = new BitmapImage(new Uri("/Resource/No_Picture.jpg", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    //아니라면 Naver Open API에서 가져온 movie.Image 값을 imgPoster에 넣어줌.
                    imgPoster.Source = new BitmapImage(new Uri(movie.Image, UriKind.RelativeOrAbsolute));
                }
            }

            if(grdResult.SelectedItem is TblFavoriteMovies) // 즐겨찾기 DB 값이면
            {
                var movie = grdResult.SelectedItem as TblFavoriteMovies;

                if (string.IsNullOrEmpty(movie.Image))
                {
                    //movie.Image에 값이 없으면 Resource에 저장된 No_Picture를 불러옴.
                    imgPoster.Source = new BitmapImage(new Uri("/Resource/No_Picture.jpg", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    //아니라면 Naver Open API에서 가져온 movie.Image 값을 imgPoster에 넣어줌.
                    imgPoster.Source = new BitmapImage(new Uri(movie.Image, UriKind.RelativeOrAbsolute));
                }
            }

        }

        /// <summary>
        /// 네이버 영화 웹브라우저 열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNaverMoive_Click(object sender, RoutedEventArgs e)
        {
            if(grdResult.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("네이버영화", "영화를 선택하세요.");
                return;
            }

            if (grdResult.SelectedItems.Count > 1)
            {
                Commons.ShowMessageAsync("네이버영화", "영화를 하나만 선택하세요.");
                return;
            }

            string linkUrl = string.Empty;
            if (IsFavorite == true)
            {
                linkUrl = (grdResult.SelectedItem as TblFavoriteMovies).Link;
            }

            else 
            {
                linkUrl = (grdResult.SelectedItem as MovieItem).Link;
            }

            Process.Start(linkUrl);
        }
    }
}
