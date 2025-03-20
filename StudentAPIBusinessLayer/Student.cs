using StudentDataAccessLayer;
using System.Diagnostics;

namespace StudentAPIBusinessLayer
{
    public class Student
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        /**
         * Gets the student data transfer object (DTO).
         * 
         * @return StudentDTO The DTO containing student details.
         */
        public StudentDTO studentDto
        {
            get
            {
                return (new StudentDTO(this.ID, this.Name, this.Age, this.Grade));
            }
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }

        /**
         * Constructor to initialize a Student object.
         * 
         * @param studentDto The DTO containing student information.
         * @param cMode The mode of the student (AddNew/Update).
         */
        public Student(StudentDTO studentDto, enMode cMode = enMode.AddNew)
        {
            this.ID = studentDto.StudentId;
            this.Name = studentDto.Name;
            this.Age = studentDto.Age;
            this.Grade = studentDto.Grade;

            Mode = cMode;
        }

        /**
         * Retrieves all students.
         * 
         * @return List<StudentDTO> A list of all students.
         */
        public static List<StudentDTO> GetAllStudent()
        {
            return StudentData.GetAllStudents();
        }

        /**
         * Retrieves all students who have passed.
         * 
         * @return List<StudentDTO> A list of students who passed.
         */
        public static List<StudentDTO> GetPassedStudent()
        {
            return StudentData.GetPassedStudents();
        }

        /**
         * Calculates the average grade of all students.
         * 
         * @return double The average grade.
         */
        public static double GetAverageGrade()
        {
            return StudentData.GetAverageGrade();
        }

        /**
         * Finds a student by ID.
         * 
         * @param id The ID of the student to find.
         * @return Student? The student object if found, otherwise null.
         */
        public static Student? Find(int id)
        {
            StudentDTO? studentDto = StudentData.GetStudentById(id);

            if (studentDto != null)
            {
                return new Student(studentDto, enMode.Update);
            }

            return null;
        }

        /**
         * Adds a new student to the database.
         * 
         * @return bool True if the student is added successfully, otherwise false.
         */
        private bool _AddNewStudent()
        {
            this.ID = StudentData.AddStudent(studentDto);
            return (this.ID != -1);
        }

        /**
         * Updates an existing student's information.
         * 
         * @return bool True if the student is updated successfully, otherwise false.
         */
        private bool _UpdateStudent()
        {
            return StudentData.UpdateStudent(studentDto);
        }

        /**
         * Saves the student record. If the mode is AddNew, it adds a student;
         * if the mode is Update, it updates the student.
         * 
         * @return bool True if the operation is successful, otherwise false.
         */
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewStudent())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateStudent();
            }

            return false;
        }

        /**
         * Deletes a student from the database.
         * 
         * @param ID The ID of the student to delete.
         * @return bool True if the student is deleted successfully, otherwise false.
         */
        public static bool DeleteStudent(int ID)
        {
            return StudentData.DeleteStudent(ID);
        }
    }
}
