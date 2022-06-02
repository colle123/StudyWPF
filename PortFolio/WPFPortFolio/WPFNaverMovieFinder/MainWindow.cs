using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFNaverMovieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
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
            }
            catch (Exception ex)
            {

                
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

        }

        private void btnViewWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btnDelWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btnWatchTrailer_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void grdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if(grdResult.SelectedItem is MovieItem)
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

            string linkUrl = (grdResult.SelectedItem as MovieItem).Link;
            Process.Start(linkUrl);
        }
    }
}
