using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using StudentAPIBusinessLayer;
using StudentDataAccessLayer;

namespace StudentAPI.Controllers
{
    [Route("api/Students")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        /**
         * Retrieves a list of all students.
         * 
         * @return A list of StudentDTO objects if students exist, otherwise a 404 Not Found response.
         */
        [HttpGet("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents()
        {
            List<StudentDTO> StudentList = StudentAPIBusinessLayer.Student.GetAllStudent();

            if (StudentList.Count == 0)
            {
                return NotFound("No Students Found!");
            }

            return Ok(StudentList);
        }

        /**
         * Retrieves a list of students who have passed.
         * 
         * @return A list of StudentDTO objects representing passed students, or 404 if none found.
         */
        [HttpGet("Passed", Name = "GetPassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetPassedStudents()
        {
            List<StudentDTO> PassedStudentList = StudentAPIBusinessLayer.Student.GetPassedStudent();

            if (PassedStudentList.Count == 0)
            {
                return NotFound("No Students Found!");
            }

            return Ok(PassedStudentList);
        }

        /**
         * Calculates the average grade of all students.
         * 
         * @return The average grade as a double.
         */
        [HttpGet("AverageGrade", Name = "GetAverageGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<double> GetAverageGrade()
        {
            double AverageGrade = StudentAPIBusinessLayer.Student.GetAverageGrade();
            return Ok(AverageGrade);
        }

        /**
         * Retrieves details of a student by their ID.
         * 
         * @param id The ID of the student to retrieve.
         * @return The StudentDTO object if found, otherwise a 404 response.
         */
        [HttpGet("{id}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StudentDTO> GetStudentById(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Invalid ID {id}");
            }

            Student? student = StudentAPIBusinessLayer.Student.Find(id);

            if (student == null)
            {
                return NotFound($"Student with ID {id} not found!");
            }
            else
            {
                return Ok(student.studentDto);
            }
        }

        /**
         * Adds a new student to the database.
         * 
         * @param newStudentDto The StudentDTO object containing new student details.
         * @return The created StudentDTO object with an assigned ID.
         */
        [HttpPost(Name = "AddNewStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> AddNewStudent(StudentDTO newStudentDto)
        {
            if (newStudentDto == null || string.IsNullOrEmpty(newStudentDto.Name) || newStudentDto.Age < 0 || newStudentDto.Grade > 100 || newStudentDto.Grade < 0)
            {
                return BadRequest("Invalid Student Data");
            }

            StudentAPIBusinessLayer.Student student = new StudentAPIBusinessLayer.Student(new StudentDTO(newStudentDto.StudentId, newStudentDto.Name, newStudentDto.Age, newStudentDto.Grade));
            student.Save();
            newStudentDto.StudentId = student.ID;
            return CreatedAtRoute("GetStudentById", new { id = newStudentDto.StudentId }, newStudentDto);
        }

        /**
         * Updates an existing student's details.
         * 
         * @param id The ID of the student to update.
         * @param updatedStudentDto The updated StudentDTO object.
         * @return The updated StudentDTO object if successful.
         */
        [HttpPut("{id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StudentDTO> UpdateStudent(int id, StudentDTO updatedStudentDto)
        {
            if (updatedStudentDto == null || string.IsNullOrEmpty(updatedStudentDto.Name) || updatedStudentDto.Age < 0 || updatedStudentDto.Grade > 100 || updatedStudentDto.Grade < 0)
            {
                return BadRequest("Invalid Student Data");
            }

            StudentAPIBusinessLayer.Student? student = StudentAPIBusinessLayer.Student.Find(id);

            if (student == null)
            {
                return NotFound($"Student with ID {id} Not Found!");
            }

            student.Name = updatedStudentDto.Name;
            student.Age = updatedStudentDto.Age;
            student.Grade = updatedStudentDto.Grade;

            if (student.Save() == false)
            {
                return StatusCode(500, new { message = "Error Updating Student" });
            }

            return Ok(student.studentDto);
        }

        /**
         * Deletes a student by ID.
         * 
         * @param id The ID of the student to delete.
         * @return A confirmation message if deletion is successful.
         */
        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteStudent(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Invalid ID {id}");
            }

            if (StudentAPIBusinessLayer.Student.DeleteStudent(id))
                return Ok($"Student with ID {id} has been deleted.");
            else
                return NotFound($"Student with ID {id} not found. No rows deleted!");
        }
    }
}
