using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace AplikasiPencatatanWarga
{
    public class DatabaseManager
    {
        private string dbPath;

        public DatabaseManager()
        {
            string appDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
            string dataFolder = Path.Combine(appDirectory, "Data");

            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            dbPath = Path.Combine(dataFolder, "db_warga.db");
            InitializeDatabase();
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);

            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                        CREATE TABLE IF NOT EXISTS tabel_warga (
                            NIK TEXT PRIMARY KEY UNIQUE NOT NULL,
                            NamaLengkap TEXT NOT NULL,
                            TanggalLahir TEXT,
                            JenisKelamin TEXT NOT NULL,
                            Alamat TEXT,
                            Pekerjaan TEXT,
                            StatusPerkawinan TEXT
                        );";
                    using (var cmd = new SQLiteCommand(query, conn))
                        cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saat inisialisasi database: {ex.Message}", "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public bool SaveWarga(string nik, string namaLengkap, DateTime tglLahir, string jenisKelamin, string alamat, string pekerjaan, string status)
        {
            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                        INSERT OR REPLACE INTO tabel_warga (NIK, NamaLengkap, TanggalLahir, JenisKelamin, Alamat, Pekerjaan, StatusPerkawinan)
                        VALUES (@nik, @namaLengkap, @tgl, @jk, @alamat, @pekerjaan, @status);";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nik", nik);
                        cmd.Parameters.AddWithValue("@namaLengkap", namaLengkap);
                        cmd.Parameters.AddWithValue("@tgl", tglLahir.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@jk", jenisKelamin);
                        cmd.Parameters.AddWithValue("@alamat", alamat);
                        cmd.Parameters.AddWithValue("@pekerjaan", pekerjaan);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error simpan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        public DataTable GetAllWarga()
        {
            var dt = new DataTable();
            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM tabel_warga ORDER BY NamaLengkap ASC;";
                    using (var adapter = new SQLiteDataAdapter(query, conn))
                        adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error ambil data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return dt;
        }

        public bool DeleteWarga(string nik)
        {
            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM tabel_warga WHERE NIK = @nik;";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nik", nik);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error hapus data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        public DataRow GetWargaByNIK(string nik)
        {
            var dt = new DataTable();
            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM tabel_warga WHERE NIK = @nik;";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nik", nik);
                        using (var adapter = new SQLiteDataAdapter(cmd))
                            adapter.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error ambil data NIK: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
    }
}
