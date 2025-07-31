using System;
using System.Data;
using System.Windows.Forms;

namespace AplikasiPencatatanWarga
{
    public partial class Form1 : Form
    {
        private DatabaseManager dbManager;
        private string selectedNIK = string.Empty;

        // Komponen UI
        TextBox txtNIK, txtNamaLengkap, txtAlamat, txtPekerjaan;
        ComboBox cmbJenisKelamin, cmbStatusPerkawinan;
        DateTimePicker dtpTanggalLahir;
        Button btnSimpan, btnReset, btnHapus, btnUbah;
        DataGridView dgvWarga;

        public Form1()
        {
            dbManager = new DatabaseManager();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            this.Text = "Aplikasi Pencatatan Data Warga";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 800;
            this.Height = 600;

            var grp = new GroupBox() { Text = "Data Warga", Left = 10, Top = 10, Width = 760, Height = 200 };

            var lblNIK = new Label() { Text = "NIK:", Left = 10, Top = 20, Width = 100 };
            txtNIK = new TextBox() { Left = 120, Top = 20, Width = 200, MaxLength = 16 };

            var lblNama = new Label() { Text = "Nama Lengkap:", Left = 10, Top = 50, Width = 100 };
            txtNamaLengkap = new TextBox() { Left = 120, Top = 50, Width = 200 };

            var lblTanggal = new Label() { Text = "Tanggal Lahir:", Left = 10, Top = 80, Width = 100 };
            dtpTanggalLahir = new DateTimePicker() { Left = 120, Top = 80, Width = 200, Format = DateTimePickerFormat.Long };

            var lblJK = new Label() { Text = "Jenis Kelamin:", Left = 10, Top = 110, Width = 100 };
            cmbJenisKelamin = new ComboBox() { Left = 120, Top = 110, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbJenisKelamin.Items.AddRange(new string[] { "Laki-laki", "Perempuan" });

            var lblAlamat = new Label() { Text = "Alamat:", Left = 350, Top = 20, Width = 100 };
            txtAlamat = new TextBox() { Left = 460, Top = 20, Width = 280, Multiline = true, Height = 40, ScrollBars = ScrollBars.Vertical };

            var lblPekerjaan = new Label() { Text = "Pekerjaan:", Left = 350, Top = 70, Width = 100 };
            txtPekerjaan = new TextBox() { Left = 460, Top = 70, Width = 280 };

            var lblStatus = new Label() { Text = "Status Perkawinan:", Left = 350, Top = 100, Width = 100 };
            cmbStatusPerkawinan = new ComboBox() { Left = 460, Top = 100, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatusPerkawinan.Items.AddRange(new string[] { "Belum Kawin", "Kawin", "Cerai Hidup", "Cerai Mati" });

            grp.Controls.AddRange(new Control[] { lblNIK, txtNIK, lblNama, txtNamaLengkap, lblTanggal, dtpTanggalLahir, lblJK, cmbJenisKelamin, lblAlamat, txtAlamat, lblPekerjaan, txtPekerjaan, lblStatus, cmbStatusPerkawinan });
            this.Controls.Add(grp);

            // Tombol Aksi
            btnSimpan = new Button() { Text = "Simpan", Left = 10, Top = 220, Width = 100 };
            btnReset = new Button() { Text = "Reset", Left = 120, Top = 220, Width = 100 };
            btnHapus = new Button() { Text = "Hapus", Left = 230, Top = 220, Width = 100, Enabled = false };
            btnUbah = new Button() { Text = "Ubah", Left = 340, Top = 220, Width = 100, Enabled = false };
            this.Controls.AddRange(new Control[] { btnSimpan, btnReset, btnHapus, btnUbah });

            // DataGridView
            dgvWarga = new DataGridView()
            {
                Left = 10,
                Top = 260,
                Width = 760,
                Height = 280,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(dgvWarga);

            // Event handler
            btnSimpan.Click += BtnSimpan_Click;
            btnReset.Click += BtnReset_Click;
            btnHapus.Click += BtnHapus_Click;
            btnUbah.Click += BtnUbah_Click;
            dgvWarga.CellClick += DgvWarga_CellClick;

            LoadDataToGrid();
        }

        private void LoadDataToGrid()
        {
            var dt = dbManager.GetAllWarga();
            dgvWarga.DataSource = dt;
            dgvWarga.ClearSelection();
            dgvWarga.AutoResizeColumns();
        }

        private void ResetForm()
        {
            txtNIK.Text = "";
            txtNamaLengkap.Text = "";
            txtAlamat.Text = "";
            txtPekerjaan.Text = "";
            dtpTanggalLahir.Value = DateTime.Now;
            cmbJenisKelamin.SelectedIndex = 0;
            cmbStatusPerkawinan.SelectedIndex = 0;
            txtNIK.ReadOnly = false;
            selectedNIK = "";
            btnHapus.Enabled = false;
            btnUbah.Enabled = false;
            dgvWarga.ClearSelection();
        }

        private void BtnSimpan_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNIK.Text) || string.IsNullOrWhiteSpace(txtNamaLengkap.Text) || cmbJenisKelamin.SelectedIndex == -1)
            {
                MessageBox.Show("Isi semua data penting!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = dbManager.SaveWarga(
                txtNIK.Text.Trim(),
                txtNamaLengkap.Text.Trim(),
                dtpTanggalLahir.Value,
                cmbJenisKelamin.Text,
                txtAlamat.Text,
                txtPekerjaan.Text,
                cmbStatusPerkawinan.Text
            );

            if (success)
            {
                MessageBox.Show("Data berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid();
                ResetForm();
            }
        }

        private void BtnReset_Click(object? sender, EventArgs e) => ResetForm();

        private void BtnHapus_Click(object? sender, EventArgs e)
        {
            if (selectedNIK == "") return;
            if (MessageBox.Show("Yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dbManager.DeleteWarga(selectedNIK);
                LoadDataToGrid();
                ResetForm();
            }
        }

        private void BtnUbah_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Silakan ubah data di form lalu klik Simpan untuk memperbarui.", "Ubah", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DgvWarga_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvWarga.Rows[e.RowIndex];
                selectedNIK = row.Cells["NIK"].Value.ToString()!;
                txtNIK.Text = selectedNIK;
                txtNamaLengkap.Text = row.Cells["NamaLengkap"].Value.ToString();
                DateTime.TryParse(row.Cells["TanggalLahir"].Value.ToString(), out var tgl);
                dtpTanggalLahir.Value = tgl;
                cmbJenisKelamin.Text = row.Cells["JenisKelamin"].Value.ToString();
                txtAlamat.Text = row.Cells["Alamat"].Value.ToString();
                txtPekerjaan.Text = row.Cells["Pekerjaan"].Value.ToString();
                cmbStatusPerkawinan.Text = row.Cells["StatusPerkawinan"].Value.ToString();

                txtNIK.ReadOnly = true;
                btnHapus.Enabled = true;
                btnUbah.Enabled = true;
            }
        }
    }
} 
