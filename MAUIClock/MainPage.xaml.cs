using System.Timers;

namespace MAUIClock
{
    public partial class MainPage : ContentPage
    {
        private DateTime _currentTime;
        private System.Timers.Timer _timer;

        public MainPage()
        {
            InitializeComponent();
            InitializeClock();
        }

        private void InitializeClock()
        {
            // Получаем текущее системное время
            _currentTime = DateTime.Now;

            // Настройка таймера для обновления каждую секунду
            _timer = new System.Timers.Timer(1000); // 1000 миллисекунд = 1 секунда
            _timer.Elapsed += UpdateClockDisplay;
            _timer.Start();

            // Первоначальное обновление дисплея
            UpdateClockDisplay(null, null);

            // Привязка события нажатия кнопки
            UpdateTimeButton.Clicked += (s, e) => ResetToCurrentTime();
        }

        private void UpdateClockDisplay(object sender, ElapsedEventArgs e)
        {
            // Увеличиваем время на одну секунду
            _currentTime = DateTime.Now;
            //_currentTime = _currentTime.AddSeconds(1);

            // Обновляем UI в основном потоке
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Разбиваем время на компоненты
                int hours = _currentTime.Hour;
                int minutes = _currentTime.Minute;
                int seconds = _currentTime.Second;

                // Обновляем дисплеи часов
                HourTens.Digit = hours / 10;
                HourOnes.Digit = hours % 10;

                // Обновляем дисплеи минут
                MinuteTens.Digit = minutes / 10;
                MinuteOnes.Digit = minutes % 10;

                // Обновляем дисплеи секунд
                SecondTens.Digit = seconds / 10;
                SecondOnes.Digit = seconds % 10;
            });
        }
        private void ResetToCurrentTime()
        {
            // Сброс к текущему системному времени
            _currentTime = DateTime.Now;
            UpdateClockDisplay(null, null);
        }
        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    _timer?.Stop();
        //    _timer?.Dispose();
        //}
    }

    public class SevenSegmentDisplay : GraphicsView
    {
        private int _digit = 0;
        private Color _activeColor = Colors.Red;
        private Color _inactiveColor = Colors.Black;

        public int Digit
        {
            get => _digit;
            set
            {
                _digit = value;
                Invalidate(); // Перерисовка при изменении цифры
            }
        }

        public Color ActiveColor
        {
            get => _activeColor;
            set
            {
                _activeColor = value;
                Invalidate();
            }
        }

        public Color InactiveColor
        {
            get => _inactiveColor;
            set
            {
                _inactiveColor = value;
                Invalidate();
            }
        }

        public SevenSegmentDisplay()
        {
            Drawable = new SevenSegmentDrawable(this);
        }

        private class SevenSegmentDrawable : IDrawable
        {
            private static readonly bool[][] DigitSegments = new bool[][]
            {
                new bool[] { true, true, true, true, true, true, false },   // 0
                new bool[] { false, true, true, false, false, false, false },// 1
                new bool[] { true, true, false, true, true, false, true },  // 2
                new bool[] { true, true, true, true, false, false, true },  // 3
                new bool[] { false, true, true, false, false, true, true }, // 4
                new bool[] { true, false, true, true, false, true, true },  // 5
                new bool[] { true, false, true, true, true, true, true },   // 6
                new bool[] { true, true, true, false, false, false, false },// 7
                new bool[] { true, true, true, true, true, true, true },    // 8
                new bool[] { true, true, true, true, false, true, true }    // 9
            };

            private readonly SevenSegmentDisplay _parent;

            public SevenSegmentDrawable(SevenSegmentDisplay parent)
            {
                _parent = parent;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                // Проверка корректности цифры
                if (_parent.Digit < 0 || _parent.Digit > 9)
                    return;

                bool[] segments = DigitSegments[_parent.Digit];
                float segmentThickness = dirtyRect.Height / 10f;

                var horizontalHeight = segmentThickness;
                var horizontalWidth = dirtyRect.Width - 2 * segmentThickness;
                var verticalHeight = (dirtyRect.Height - 3 * segmentThickness) / 2;
                var verticalWidth = segmentThickness;

                // Координаты сегментов
                var segmentCoordinates = new[]
                {
                    // Верхний горизонтальный
                    new[] { dirtyRect.X + segmentThickness, dirtyRect.Y, horizontalWidth, horizontalHeight },
                    // Верхний правый вертикальный
                    new[] { dirtyRect.X + dirtyRect.Width - segmentThickness, dirtyRect.Y + segmentThickness, verticalWidth, verticalHeight },
                    // Нижний правый вертикальный
                    new[] { dirtyRect.X + dirtyRect.Width - segmentThickness, dirtyRect.Y + segmentThickness * 2 + verticalHeight, verticalWidth, verticalHeight },
                    // Нижний горизонтальный
                    new[] { dirtyRect.X + segmentThickness, dirtyRect.Y + dirtyRect.Height - segmentThickness, horizontalWidth, horizontalHeight },
                    // Нижний левый вертикальный
                    new[] { dirtyRect.X, dirtyRect.Y + segmentThickness * 2 + verticalHeight, verticalWidth, verticalHeight },
                    // Верхний левый вертикальный
                    new[] { dirtyRect.X, dirtyRect.Y + segmentThickness, verticalWidth, verticalHeight },
                    // Центральный горизонтальный
                    new[] { dirtyRect.X + segmentThickness, dirtyRect.Y + segmentThickness + verticalHeight, horizontalWidth, horizontalHeight }
                };

                for (int i = 0; i < segmentCoordinates.Length; i++)
                {
                    canvas.FillColor = segments[i] ? _parent.ActiveColor : _parent.InactiveColor;
                    canvas.FillRectangle(
                        segmentCoordinates[i][0],
                        segmentCoordinates[i][1],
                        segmentCoordinates[i][2],
                        segmentCoordinates[i][3]
                    );
                }
            }
        }
    }
}
