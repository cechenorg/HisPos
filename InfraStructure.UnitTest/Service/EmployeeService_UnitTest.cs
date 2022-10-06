using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His_Pos.NewClass.Person.Employee;
using NSubstitute;

namespace InfraStructure.UnitTest.Service
{
    /// <summary>
    /// Summary description for EmployeeService_UnitTest
    /// </summary>
    [TestClass]
    public class EmployeeService_UnitTest
    {

        private EmployeeService employeeService;
        private IEmployeeDB employeeDb;
        public EmployeeService_UnitTest()
        {
            employeeDb = Substitute.For<IEmployeeDB>();
            employeeService = new EmployeeService(employeeDb);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GetData()
        { 
            List<Employee> empList = new List<Employee>();
            empList.Add(new Employee(){ID = 123});
            employeeDb.GetData().Returns(empList);

            var result = employeeService.GetData();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(123, result.Single().ID);
            employeeDb.Received(1).GetData();
        }

        [TestMethod]
        public void GetDataByID()
        {
            List<Employee> empList = new List<Employee>();
            empList.Add(new Employee() { ID = 123 });
            employeeDb.GetData().Returns(empList);

            var wrongResult = employeeService.GetDataByID(1);
            var correctResult = employeeService.GetDataByID(123);

            Assert.AreEqual(null,wrongResult);
            Assert.AreEqual(123, correctResult.ID);
        }

    }
}
