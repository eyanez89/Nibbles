using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nibbles.Models;

namespace Nibbles.Controllers
{
    public class NibblesController : Controller
    {
        public Snake Snake { get; set; }
        public Space Space { get; set; }
        public List<Snake> Snakes { get; set; }

        public int AvailableMovesUp { get; set; }
        public int AvailableMovesDown { get; set; }
        public int AvailableMovesLeft { get; set; }
        public int AvailableMovesRight { get; set; }

        // POST api/values
        [HttpPost("{Request}")]
        [Route("/NextMove")]
        public async Task<Response> MoveNext([FromBody] Request request)
        {
            //Snake = new Snake(request.Snake.Id)
            //{
            //    X = request.Snake.X,
            //    Y = request.Snake.Y,
            //    Ticks = request.Snake.Ticks,
            //    Trail = request.Snake.Trail,
            //    Direccion = (Direction)Enum.Parse(typeof(Direction), request.Snake.Direction)
            //};

            //Space = new Space(request.Space.TopX, request.Space.TopY)
            //{
            //    Map = request.Space.Map
            //};

            Snake = request.Snake;
            Space = request.Space;
            Snakes = request.Snakes;
            Snake.Direccion = (Direction)Enum.Parse(typeof(Direction), request.Snake.Direction);

            var tasks = new List<Task>
            {
                Task.Run(() => VerifyDirectionUp()),
                Task.Run(() => VerifyDirectionLeft()),
                Task.Run(() => VerifyDirectionDown()),
                Task.Run(() => VerifyDirectionRight())
            };

            await Task.WhenAll(tasks);

            if (Snake.IsOpositeDirection(Direction.Down))
                AvailableMovesDown = -1;
            if (Snake.IsOpositeDirection(Direction.Left))
                AvailableMovesLeft = -1;
            if (Snake.IsOpositeDirection(Direction.Up))
                AvailableMovesUp = -1;
            if (Snake.IsOpositeDirection(Direction.Right))
                AvailableMovesRight = -1;

            if(Snake.Direccion == Direction.Down && AvailableMovesDown > 2)
                return new Response() { Direction = Direction.Down.ToString() };
            if (Snake.Direccion == Direction.Left && AvailableMovesLeft > 2)
                return new Response() { Direction = Direction.Left.ToString() };
            if (Snake.Direccion == Direction.Up && AvailableMovesUp > 2)
                return new Response() { Direction = Direction.Up.ToString() };
            if (Snake.Direccion == Direction.Right && AvailableMovesRight > 2)
                return new Response() { Direction = Direction.Right.ToString() };


            if (!Snake.IsOpositeDirection(Direction.Down) && IsMaxValue(AvailableMovesDown, AvailableMovesLeft, AvailableMovesUp, AvailableMovesRight))
                return new Response() { Direction = Direction.Down.ToString() };

            if (!Snake.IsOpositeDirection(Direction.Left) && IsMaxValue(AvailableMovesLeft, AvailableMovesDown, AvailableMovesUp, AvailableMovesRight))
                return new Response() { Direction = Direction.Left.ToString() };

            if (!Snake.IsOpositeDirection(Direction.Up) && IsMaxValue(AvailableMovesUp, AvailableMovesDown, AvailableMovesLeft, AvailableMovesRight))
                return new Response() { Direction = Direction.Up.ToString() };

            if (!Snake.IsOpositeDirection(Direction.Right) && IsMaxValue(AvailableMovesRight, AvailableMovesDown, AvailableMovesLeft, AvailableMovesUp))
                return new Response() { Direction = Direction.Right.ToString() };

            return new Response() { Direction = Snake.Direccion.ToString() };
        }

        public bool IsMaxValue(int value, int compare1, int compare2, int compare3)
        {
            return (value >= compare1 && value >= compare2 && value >= compare3);
        }

        public void VerifyDirectionUp()
        {
            AvailableMovesUp = 0;

            if (Snake.Y == Space.TopY)
                return;

            var posY = Snake.Y;

            var posX = Snake.X;
            var posX_plus = posX;
            var posX_less = posX;

            if (posX != 0)
                posX_less = posX - 1;

            if (posX != Space.TopX - 1)
                posX_plus = posX + 1;

            var found = false;

            posY--;

            while (!found && posY > -1)
            {
                var previous = Space.Map[posX_less, posY];
                var actual = Space.Map[posX, posY];
                var next = Space.Map[posX_plus, posY];

                if (previous != 0 && Snakes.Any(s => s.Id == previous && s.X == posX_less && s.Y == posY && s.Direction == "Right"))
                {
                    found = true;
                    continue;
                }

                if (next != 0 && Snakes.Any(s => s.Id == next && s.X == posX_plus && s.Y == posY && s.Direction == "Left"))
                {
                    found = true;
                    continue;
                }

                if (actual == 0)
                    AvailableMovesUp++;
                else
                    found = true;

                posY--;
            }
        }

        public void VerifyDirectionLeft()
        {
            AvailableMovesLeft = 0;

            if (Snake.X == 0)
                return;

            var posX = Snake.X;

            var posY = Snake.Y;
            var posY_plus = posY;
            var posY_less = posY;

            if (posY != 0)
                posY_less = posY - 1;

            if (posY != Space.TopY - 1)
                posY_plus = posY + 1;

            var found = false;

            posX--;

            while (!found && posX > -1)
            {
                var previous = Space.Map[posX, posY_less];
                var actual = Space.Map[posX, posY];
                var next = Space.Map[posX, posY_plus];

                if (previous != 0 && Snakes.Any(s => s.Id == previous && s.X == posX && s.Y == posY_less && s.Direction == "Down"))
                {
                    found = true;
                    continue;
                }

                if (next != 0 && Snakes.Any(s => s.Id == next && s.X == posX && s.Y == posY_plus && s.Direction == "Up"))
                {
                    found = true;
                    continue;
                }

                if (actual == 0)
                    AvailableMovesLeft++;
                else
                    found = true;

                posX--;
            }
        }

        public void VerifyDirectionDown()
        {
            AvailableMovesDown = 0;

            if (Snake.Y == 0)
                return;

            var posY = Snake.Y;

            var posX = Snake.X;
            var posX_plus = posX;
            var posX_less = posX;

            if (posX != 0)
                posX_less = posX - 1;

            if (posX != Space.TopX - 1)
                posX_plus = posX + 1;

            var found = false;

            posY++;

            while (!found && posY < Space.TopY)
            {
                var previous = Space.Map[posX_less, posY];
                var actual = Space.Map[posX, posY];
                var next = Space.Map[posX_plus, posY];

                if (previous != 0 && Snakes.Any(s => s.Id == previous && s.X == posX_less && s.Y == posY && s.Direction == "Right"))
                {
                    found = true;
                    continue;
                }

                if (next != 0 && Snakes.Any(s => s.Id == next && s.X == posX_plus && s.Y == posY && s.Direction == "Left"))
                {
                    found = true;
                    continue;
                }

                if (actual == 0)
                    AvailableMovesDown++;
                else
                    found = true;

                posY++;
            }
        }

        public void VerifyDirectionRight()
        {
            AvailableMovesRight = 0;

            if (Snake.X == Space.TopX)
                return;

            var posX = Snake.X;

            var posY = Snake.Y;
            var posY_plus = posY;
            var posY_less = posY;

            if (posY != 0)
                posY_less = posY - 1;

            if (posY != Space.TopY - 1)
                posY_plus = posY + 1;

            var found = false;

            posX++;

            while (!found && posX < Space.TopX)
            {
                var previous = Space.Map[posX, posY_less];
                var actual = Space.Map[posX, posY];
                var next = Space.Map[posX, posY_plus];

                if (previous != 0 && Snakes.Any(s => s.Id == previous && s.X == posX && s.Y == posY_less && s.Direction == "Down"))
                {
                    found = true;
                    continue;
                }

                if (next != 0 && Snakes.Any(s => s.Id == next && s.X == posX && s.Y == posY_plus && s.Direction == "Up"))
                {
                    found = true;
                    continue;
                }

                if (actual == 0)
                    AvailableMovesRight++;
                else
                    found = true;

                posX++;
            }
        }
    }
}
