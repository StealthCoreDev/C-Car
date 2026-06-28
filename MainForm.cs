using System.Drawing.Drawing2D;

namespace StopCarApp
{
    public class MainForm : Form
    {
        // --- تنظیمات قابل تغییر ---
        private const int CarWidth = 90;
        private const int CarHeight = 40;
        private const int ConeSize = 36;
        private const float CarSpeed = 4.5f;          // سرعت حرکت ماشین (پیکسل در هر فریم)
        private const float SafeStopDistance = 70f;   // فاصله‌ای که ماشین باید قبل از مخروط متوقف شود

        // --- وضعیت صحنه ---
        private float carX;                // موقعیت افقی جلوی ماشین
        private readonly int carY;         // موقعیت عمودی (ثابت، روی خط جاده)
        private readonly Point conePosition;
        private bool isButtonHeld;         // آیا دکمه نگه داشته شده؟
        private bool isCarMoving;          // آیا ماشین واقعاً در حال حرکت است؟
        private string statusText = "برای حرکت، دکمه را نگه دارید";

        private readonly System.Windows.Forms.Timer gameTimer;
        private readonly Button moveButton;
        private readonly Button resetButton;
        private readonly Label statusLabel;

        public MainForm()
        {
            // --- تنظیمات پنجره ---
            this.Text = "ماشین و مخروط ایست - StopCarApp v1.0";
            this.ClientSize = new Size(900, 400);
            this.DoubleBuffered = true; // برای جلوگیری از پرش/فلیکر هنگام رسم
            this.BackColor = Color.FromArgb(235, 235, 235);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // موقعیت اولیه: ماشین سمت چپ، مخروط نزدیک سمت راست
            carY = this.ClientSize.Height / 2 + 40;
            carX = 60;
            conePosition = new Point(this.ClientSize.Width - 140, carY);

            // --- دکمه‌ی حرکت (نگه‌داشتنی) ---
            moveButton = new Button
            {
                Text = "نگه دارید تا حرکت کند ▶",
                Size = new Size(220, 50),
                Location = new Point(30, 20),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            moveButton.FlatAppearance.BorderSize = 0;
            moveButton.MouseDown += (s, e) => { isButtonHeld = true; };
            moveButton.MouseUp += (s, e) => { isButtonHeld = false; };
            moveButton.MouseLeave += (s, e) => { isButtonHeld = false; }; // اگر موس از روی دکمه خارج شد هم بایستد
            this.Controls.Add(moveButton);

            // --- دکمه‌ی بازنشانی صحنه ---
            resetButton = new Button
            {
                Text = "شروع دوباره ↺",
                Size = new Size(140, 50),
                Location = new Point(270, 20),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(230, 230, 230),
                FlatStyle = FlatStyle.Flat
            };
            resetButton.Click += (s, e) => ResetScene();
            this.Controls.Add(resetButton);

            // --- نمایش وضعیت ---
            statusLabel = new Label
            {
                Text = statusText,
                AutoSize = false,
                Size = new Size(420, 50),
                Location = new Point(430, 20),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DimGray
            };
            this.Controls.Add(statusLabel);

            // --- تایمر بازی: هر فریم منطق حرکت را بررسی می‌کند ---
            gameTimer = new System.Windows.Forms.Timer { Interval = 16 }; // تقریباً 60 فریم بر ثانیه
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            float frontOfCar = carX + CarWidth;
            float distanceToCone = (conePosition.X - ConeSize / 2f) - frontOfCar;

            if (isButtonHeld)
            {
                if (distanceToCone > SafeStopDistance)
                {
                    // هنوز فاصله کافی هست -> حرکت کن
                    carX += CarSpeed;
                    isCarMoving = true;
                    statusText = "در حال حرکت...";
                }
                else
                {
                    // به فاصله‌ی امن نزدیک مخروط رسیدیم -> توقف خودکار
                    isCarMoving = false;
                    statusText = "ایست! نزدیک مخروط متوقف شد ✋";
                }
            }
            else
            {
                // دکمه رها شده -> ماشین فوراً می‌ایستد
                isCarMoving = false;
                if (distanceToCone <= SafeStopDistance)
                    statusText = "ایست! نزدیک مخروط متوقف شد ✋";
                else
                    statusText = "متوقف شد (دکمه رها شد)";
            }

            statusLabel.Text = statusText;
            statusLabel.ForeColor = isCarMoving ? Color.FromArgb(20, 130, 20) : Color.DarkRed;

            this.Invalidate(); // درخواست رسم دوباره صحنه
        }

        private void ResetScene()
        {
            carX = 60;
            isButtonHeld = false;
            isCarMoving = false;
            statusText = "برای حرکت، دکمه را نگه دارید";
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int roadY = carY + CarHeight + 6;

            // --- رسم جاده ---
            using (var roadBrush = new SolidBrush(Color.FromArgb(70, 70, 70)))
                g.FillRectangle(roadBrush, 0, roadY, this.ClientSize.Width, 8);

            // خط‌چین وسط جاده
            using (var dashPen = new Pen(Color.White, 2) { DashStyle = DashStyle.Dash })
                g.DrawLine(dashPen, 0, roadY + 4, this.ClientSize.Width, roadY + 4);

            // --- رسم مخروط ایستگاهی ---
            DrawCone(g, conePosition.X, carY);

            // --- رسم ماشین ---
            DrawCar(g, (int)carX, carY);
        }

        private void DrawCar(Graphics g, int x, int y)
        {
            // بدنه‌ی ماشین
            using var bodyBrush = new SolidBrush(Color.FromArgb(220, 60, 60));
            var bodyRect = new Rectangle(x, y, CarWidth, CarHeight);
            g.FillRoundedRectangle(bodyBrush, bodyRect, 10);

            // کابین (شیشه)
            using var cabinBrush = new SolidBrush(Color.FromArgb(180, 220, 235));
            var cabinRect = new Rectangle(x + 18, y - 14, CarWidth - 45, 20);
            g.FillRoundedRectangle(cabinBrush, cabinRect, 6);

            // چرخ‌ها
            using var wheelBrush = new SolidBrush(Color.Black);
            g.FillEllipse(wheelBrush, x + 12, y + CarHeight - 10, 20, 20);
            g.FillEllipse(wheelBrush, x + CarWidth - 32, y + CarHeight - 10, 20, 20);

            using var hubBrush = new SolidBrush(Color.LightGray);
            g.FillEllipse(hubBrush, x + 18, y + CarHeight - 4, 8, 8);
            g.FillEllipse(hubBrush, x + CarWidth - 26, y + CarHeight - 4, 8, 8);
        }

        private void DrawCone(Graphics g, int centerX, int carBaselineY)
        {
            int baseY = carBaselineY + CarHeight - 4;
            int topY = baseY - ConeSize;

            using var coneBrush = new SolidBrush(Color.FromArgb(255, 120, 30));
            var points = new[]
            {
                new Point(centerX, topY),
                new Point(centerX - ConeSize / 2, baseY),
                new Point(centerX + ConeSize / 2, baseY)
            };
            g.FillPolygon(coneBrush, points);

            // خط سفید روی مخروط (مثل مخروط‌های واقعی)
            using var whitePen = new Pen(Color.White, 4);
            int stripeY = baseY - ConeSize / 3;
            int stripeHalfWidth = (baseY - stripeY) * ConeSize / (2 * ConeSize);
            g.DrawLine(whitePen, centerX - 8, stripeY, centerX + 8, stripeY);

            // پایه مخروط
            using var baseBrush = new SolidBrush(Color.FromArgb(40, 40, 40));
            g.FillRectangle(baseBrush, centerX - ConeSize / 2 - 4, baseY, ConeSize + 8, 6);
        }
    }

    // متد کمکی برای رسم مستطیل با گوشه‌ی گرد (در GDI+ پیش‌فرض وجود ندارد)
    internal static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            g.FillPath(brush, path);
        }
    }
}
