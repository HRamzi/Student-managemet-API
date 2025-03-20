using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace StudentDataAccessLayer
{
    /**
     * Represents a Data Transfer Object (DTO) for a Student.
     * Contains properties for student details.
     */
    public class StudentDTO
    {
        /**
         * Initializes a new instance of the StudentDTO class.
         * 
         * @param id The unique identifier of the student.
         * @param name The name of the student.
         * @param age The age of the student.
         * @param grade The grade of the student.
         */
        public StudentDTO(int id, string name, int age, int grade)
        {
            this.Id = id;
            this.Name = name;
            this.Age = age;
            this.Grade = grade;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }
        public int StudentId { get; set; }
    }

    public class StudentData
    {
        static string _connectionString = "Server=localhost;Database=StudentsDB;User Id=sa;Password=123456;TrustServerCertificate=True;";

        /**
         * Retrieves all students from the database.
         * 
         * @return A list of StudentDTO objects.
         */
        public static List<StudentDTO> GetAllStudents()
        {
            var StudentsList = new List<StudentDTO>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllStudents", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentsList.Add(new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))
                            ));
                        }
                    }
                }
                return StudentsList;
            }
        }

        /**
         * Retrieves students who have passed.
         * 
         * @return A list of StudentDTO objects representing passed students.
         */
        public static List<StudentDTO> GetPassedStudents()
        {
            var StudentsList = new List<StudentDTO>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetPassedStudents", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentsList.Add(new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))
                            ));
                        }
                    }
                }
                return StudentsList;
            }
        }

        /**
         * Retrieves the average grade of all students.
         * 
         * @return The average grade as a double.
         */
        public static double GetAverageGrade()
        {
            double averageGrade = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAverageGrade", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        averageGrade = Convert.ToDouble(result);
                    }
                }
            }
            return averageGrade;
        }

        /**
         * Finds and retrieves a student by ID.
         * 
         * @param studentId The ID of the student to retrieve.
         * @return A StudentDTO object if found, otherwise null.
         */
        public static StudentDTO? GetStudentById(int studentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetStudentById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StudentId", studentId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new StudentDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("Id")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            reader.GetInt32(reader.GetOrdinal("Age")),
                            reader.GetInt32(reader.GetOrdinal("Grade"))
                        );
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /**
         * Adds a new student to the database.
         * 
         * @param StudentDTO The student details.
         * @return The ID of the newly added student.
         */
        public static int AddStudent(StudentDTO StudentDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_AddStudent", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Name", StudentDTO.Name);
                command.Parameters.AddWithValue("@Age", StudentDTO.Age);
                command.Parameters.AddWithValue("@Grade", StudentDTO.Grade);
                var outputIdParam = new SqlParameter("@NewStudentId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(outputIdParam);
                connection.Open();
                command.ExecuteNonQuery();
                return (int)outputIdParam.Value;
            }
        }
    }
}
