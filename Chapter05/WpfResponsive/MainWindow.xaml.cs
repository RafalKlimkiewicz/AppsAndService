﻿using System.Diagnostics;
using System.Windows;
using Microsoft.Data.SqlClient;


namespace WpfResponsive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString;

        private string sql = "WAITFOR DELAY '00:00:05'; SELECT EmployeeId, FirstName, LastName FROM Employees";

        public MainWindow()
        {
            InitializeComponent();

            SqlConnectionStringBuilder builder = new()
            {
                DataSource = ".",
                InitialCatalog = "Northwind",
                Encrypt = false,
                MultipleActiveResultSets = true,
                ConnectTimeout = 5,
                IntegratedSecurity = true
            };

            connectionString = builder.ConnectionString;
        }

        private void GetEmployeesSyncButton_Click(object sender, RoutedEventArgs e)
        {
            var timer = Stopwatch.StartNew();

            using var connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();

                SqlCommand command = new(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string employee = string.Format("{0}: {1} {2}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2));

                    EmployeesListBox.Items.Add(employee);
                }

                reader.Close();
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            EmployeesListBox.Items.Add($"Sync: {timer.ElapsedMilliseconds:N0}ms");
        }

        private async void GetEmployeesAsyncButton_Click(object sender, RoutedEventArgs e)
        {
            var timer = Stopwatch.StartNew();

            using SqlConnection connection = new(connectionString);

            try
            {
                await connection.OpenAsync();

                SqlCommand command = new(sql, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string employee = string.Format("{0}: {1} {2}",
                        await reader.GetFieldValueAsync<int>(0),
                        await reader.GetFieldValueAsync<string>(1),
                        await reader.GetFieldValueAsync<string>(2));

                    EmployeesListBox.Items.Add(employee);
                }

                await reader.CloseAsync();
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            EmployeesListBox.Items.Add($"Async: {timer.ElapsedMilliseconds:N0}ms");
        }
    }
}
