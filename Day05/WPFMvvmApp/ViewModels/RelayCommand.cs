using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFMvvmApp.ViewModels
{
    public class RelayCommand<T> : ICommand // ViewModel과 View를 Glue하기 위한 Class
    {
        private readonly Action<T> execute; // 실행처리를 위한 Generic
        private readonly Predicate<T> canExcute; // 실행여부를 판단하는 Generic

        // 이벤트 핸들러의 실행여부에 따라서 이벤트를 추가해주거나 삭제하는 이벤트 핸들러
        public event EventHandler CanExecuteChanged 
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter) // 명령을 실행 할 수 있는지? -> CanExecute가 활성화 되야만 실행되는 프로그램을 만들게
        {
            return canExcute?.Invoke((T)parameter) ?? true; // 입력으로 들어온 parameter을 Generic으로 바꿔서 canExcute를 실행,
                                                            // canExecute가 null이 아니라 실행되면 true를 반환
                                                            // if(canExecute != null) canExecute(this, PropertyChanged((T)parameter));
        }

        public void Execute(object parameter) // 명령이 있는지?
        {
            execute((T)parameter); // 입력으로 들어온 parameter을 Generic으로 바꿔서 execute를 싱행
        }

        /// <summary>
        /// execute만 parameter로 받는 생성자
        /// </summary>
        /// <param name="execute"></param>
        /// 
        public RelayCommand(Action<T> execute) : this(execute, null) { }

        /// <summary>
        /// execute, canExcute를 parameter로 받는 생성자
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExcute"></param>
        public RelayCommand(Action<T> execute, Predicate<T> canExcute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExcute = canExcute;
        }
    }
}
