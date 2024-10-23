using Microsoft.AspNetCore.Http.HttpResults;

namespace api.Tests;

[ApiController]
[Route("api/[controller]")]
public class PropController : BaseApiController
{
    [HttpPost("car-speed")]
    public ActionResult<int> SetGetCarSpeed()
    {
        #region Parameter-less

        Car car0 = new();

        // return car0.speed;

        #endregion

        #region 1 Parameter

        // Car car1 = new(speedInput);

        // return car1.speed;

        #endregion

        #region 2 or more Parameters

        // Car car2 = new(30, 55, 0);

        // return car2.speed;

        #endregion

        #region Property

        Car car = new();

        // car.Speed = speedInput;

        car.Age = 30;

        return Ok(car.Age);

        // car.Speed = 500;

        // return car.Speed;

        #endregion
    }
}

// public class Car
// {
//     public int speed; // field

//     #region Parameter-less
//     public Car()
//     {
//         speed = 200;
//     }
//     #endregion

//     #region 1 Parameter
//     public Car(int speedInput)
//     {
//         speed = speedInput;
//     }
//     #endregion

//     #region 2 or more Parameters
//     public Car(int speed1, int speed2, int speed3)
//     {
//         speed = (speed1 + speed2) / speed3;
//     }
//     #endregion

//     #region 2 or more Parameters, string & bool
//     public Car(string name, bool isAlive)
//     {

//     }
//     #endregion 
// }

public class Car
{
    private int speed = 10;
    public int Speed
    {
        get { return speed; }
        set
        {
            if (value > 450)
                speed = 450;
            else
                speed = value;
        }
    }

    public int Age { get; set; } = 35;
    public string Name { get; set; } = "Parsa";
}
