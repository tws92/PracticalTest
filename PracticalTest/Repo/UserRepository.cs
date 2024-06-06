using Newtonsoft.Json;
using NLog;
using PracticalTest.Extension;
using PracticalTest.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PracticalTest.Repo
{
    public class UserRepository
    {
        private string _connectionString;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public UserRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        public void InsertUser(UsersModel users)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand("InsertUser", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                // Add parameters
                                command.Parameters.AddWithValue("@NRIC", users.NRIC);
                                command.Parameters.AddWithValue("@Name", users.Name);
                                command.Parameters.AddWithValue("@Gender", users.Gender);
                                command.Parameters.AddWithValue("@Birthday", users.Birthday);
                                command.Parameters.AddWithValue("@Age", users.Age);
                                command.Parameters.AddWithValue("@AvailableDate", users.AvailableDate ?? (object)DBNull.Value);
                                SqlParameter subjectsParam = command.Parameters.AddWithValue("@Subjects", users.Subjects.ToDataTable());

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            // Rollback the transaction if an error occurs
                            transaction.Rollback();
                            throw; // Rethrow the exception to be handled globally
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while updating user details: {Message}", ex.Message);
            }
        }

        public List<UsersModel> GetUser(string NameORNRIC, int userId)
        {
            List<UsersModel> usersList = new List<UsersModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GettUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NameOrNRIC", NameORNRIC);
                        command.Parameters.AddWithValue("@Id", userId);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsersModel user = new UsersModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                    NRIC = reader["NRIC"].ToString(),
                                    Name = reader["NAME"].ToString(),
                                    Gender = reader["GENDER"].ToString(),
                                    Birthday = reader.GetDateTime(reader.GetOrdinal("Birthday")),
                                    Age = reader.GetInt32(reader.GetOrdinal("AGE")),
                                    AvailableDate = reader.IsDBNull(reader.GetOrdinal("AvailableDate"))
                                                ? (DateTime?)null
                                                : reader.GetDateTime(reader.GetOrdinal("AvailableDate")),
                                    SelectedSubjectList = !reader.IsDBNull(reader.GetOrdinal("Subjects"))
                                           ? JsonConvert.DeserializeObject<List<SubjectsModel>>(reader["Subjects"].ToString())
                                           : new List<SubjectsModel>(),
                                };
                                usersList.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while updating user details: {Message}", ex.Message);
            }

            return usersList;
        }

        public void UpdateUser(UsersModel user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand("UpdateUser", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@Id", user.Id);
                                command.Parameters.AddWithValue("@NRIC", user.NRIC);
                                command.Parameters.AddWithValue("@Name", user.Name);
                                command.Parameters.AddWithValue("@Gender", user.Gender);
                                command.Parameters.AddWithValue("@Birthday", user.Birthday);
                                command.Parameters.AddWithValue("@Age", user.Age);
                                command.Parameters.AddWithValue("@AvailableDate", (object)user.AvailableDate ?? DBNull.Value);
                                command.Parameters.AddWithValue("@Subjects", user.Subjects.ToDataTable());

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            // Rollback the transaction if an error occurs
                            transaction.Rollback();
                            throw; // Rethrow the exception to be handled globally
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while updating user details: {Message}", ex.Message);
            }
        }

        public List<SubjectsModel> GetAllSubjects()
        {
            List<SubjectsModel> subjectsList = new List<SubjectsModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GettAllSubjects", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SubjectsModel subject = new SubjectsModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Subject = reader["Subject"].ToString(),
                                };
                                subjectsList.Add(subject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while updating user details: {Message}", ex.Message);
            }

            return subjectsList;
        }

        public void InsertAuditLog(int userId, string prevVal, string afterVal)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand("InsertAuditLog", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                // Add parameters
                                command.Parameters.AddWithValue("@UserId", userId);
                                command.Parameters.AddWithValue("@PrevVal", prevVal);
                                command.Parameters.AddWithValue("@afterVal", afterVal);

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            // Rollback the transaction if an error occurs
                            transaction.Rollback();
                            throw; // Rethrow the exception to be handled globally
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while updating user details: {Message}", ex.Message);
            }
        }
    }
}