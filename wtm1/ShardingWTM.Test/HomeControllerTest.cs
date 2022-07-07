using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;
using ShardingWTM;
using ShardingWTM.Controllers;
using ShardingWTM.ViewModels.HomeVMs;

namespace ShardingWTM.Test
{
    [TestClass]
    public class HomeControllerTest
    {
        private HomeController _controller;
        private string _seed;

        public HomeControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<HomeController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void PIndexTest()
        {
            ViewResult rv = (ViewResult)_controller.PIndex();
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void FrontPageTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.FrontPage();
            Assert.IsNotNull(rv);
        }
    }
}
