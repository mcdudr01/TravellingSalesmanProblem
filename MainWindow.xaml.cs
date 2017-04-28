using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace TravellingSalesmanProblem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Basic datastructure to store city information
        public class City
        {
            public City(int citnum, double citx, double city)
            {
                this.CitNum = citnum;
                this.citX = citx;
                this.citY = city;
            }

            private int citNum;
            private double citX;
            private double citY;

            public int CitNum
            {
                get { return citNum; }
                set { citNum = value; }
            }

            public double CitX
            {
                get { return citX; }
                set { citX = value; }
            }

            public double CitY
            {
                get { return citY; }
                set { citY = value; }
            }
        }

        //Datastructure to store Hamiltonian cycles
        public class hamilCycle
        {
            public hamilCycle(int[] Cycle, double PathWeight)
            {
                this.cycle = Cycle;
                this.pathWeight = PathWeight;
            }

            private int[] cycle;
            private double pathWeight;

            public int[] Cycle
            {
                get { return cycle; }
                set { cycle = value; }
            }

            public double PathWeight
            {
                get { return pathWeight; }
                set { pathWeight = value; }
            }
        }

        //get all cities from file
        public List<City> getCities(string file)
        {
            // Read in a file line-by-line, and store it all in a List.
            //string path2 = @"c:\temp\Random10.tsp";//get this file input dynamically later (via wpf ui File Select)
            string path2 = @file;
            List<string> list = new List<string>();
            List<City> citList = new List<City>();
            using (StreamReader reader = new StreamReader(path2))
            {
                int x = 0;
                string line;
                char[] delims = { ' ' };
                while ((line = reader.ReadLine()) != null)
                {
                    string[] temp = line.Split(delims);
                    // use TryParse to avoid needing exception handling here
                    x = 0;
                    if (Int32.TryParse(temp[0], out x))
                    {
                        // you know that the parsing attempt was successful
                        list.Add(line);
                        citList.Add(new City(Int32.Parse(temp[0]), Double.Parse(temp[1]), Double.Parse(temp[2])));
                    }
                }
            }
            return citList;
        }

        //draws all cities obtained from file
        public void drawCities(List<City> citList)
        {
            //loop here to draw points of cities, assuming list is not empty
            //put this logic in the general method that gets cities.
            Canvas myCanvas = canv1;
            canv1.Children.Clear();
            foreach (City c in citList)
            {
                Line myPoint = new Line();
                myPoint.Stroke = System.Windows.Media.Brushes.Red;
                myPoint.X1 = c.CitX * 2;
                myPoint.X2 = (c.CitX * 2) + 1;
                myPoint.Y1 = c.CitY * 2;
                myPoint.Y2 = (c.CitY * 2) + 1;
                myPoint.HorizontalAlignment = HorizontalAlignment.Left;
                myPoint.VerticalAlignment = VerticalAlignment.Center;
                myPoint.StrokeThickness = 2;
                myCanvas.Children.Add(myPoint);
            }
        }

        //used for drawing hamiltonian cycle solutions
        public void drawCycle(List<City> citList, hamilCycle hc)
        {
            Canvas myCanvas = canv1;
            var cities = citList;

            for (int x = 0; x < hc.Cycle.Length - 1; x++)
            {
                Line myPoint = new Line();
                myPoint.Stroke = System.Windows.Media.Brushes.Green;
                myPoint.X1 = cities[hc.Cycle[x] - 1].CitX * 2;
                myPoint.X2 = cities[hc.Cycle[x + 1] - 1].CitX * 2;
                myPoint.Y1 = cities[hc.Cycle[x] - 1].CitY * 2;
                myPoint.Y2 = cities[hc.Cycle[x + 1] - 1].CitY * 2;
                myPoint.HorizontalAlignment = HorizontalAlignment.Left;
                myPoint.VerticalAlignment = VerticalAlignment.Center;
                myPoint.StrokeThickness = 2;
                myCanvas.Children.Add(myPoint);
            }
        }

        #region PROJECT 1 STUFF
        /// <summary>
        /// Iterative Heap's algorithm.
        /// Takes an array of city nodes and permutes them.
        /// Returns hamilCycle of shortest path (smallest weight)
        /// </summary>
        public hamilCycle bruteForceTSP(int n, List<City> citList)
        {
            //object to store best route
            hamilCycle currBest;
            //object to store current route
            hamilCycle currRoute;

            //Since our end result is a cycle, it doesn't
            //matter where the path starts, so we can ignore
            //the first node and trim the number of permutations.
            int[] B = new int[n-1];
            for (int i = 0; i < n-1; i++)
            {
                B[i] = citList[i+1].CitNum;
            }
            
            int[] c = new int[n-1];
            for (int i = 0; i < n-1; i++)
            {
                c[i] = 0;
            }

            //set initial route as current best cycle
            currBest = calcCycleAndWeight(B, citList);

            for (int i = 1; i < n-1;)
            {
                if (c[i] < i)
                {
                    //if I is Even
                    if (i%2==0)
                    {
                        int tmp = B[0];
                        B[0] = B[i];
                        B[i] = tmp;
                    }
                    else
                    {
                        int tmp = B[c[i]];
                        B[c[i]] = B[i];
                        B[i] = tmp;
                    }

                    //New permutation here, first check if
                    //first node < last node. We do this
                    //because we don't want to consider
                    //symmetric paths. If first < last,
                    //compare against the currBest Path.
                    if (B[0] < B[n-2])
                    {
                        //calcCycleAndWeight
                        currRoute = calcCycleAndWeight(B, citList);
                        //if the current route is shorter, set it as best.
                        if (currRoute.PathWeight < currBest.PathWeight)
                        {
                            currBest = currRoute;
                        }
                    }

                    c[i] += 1;
                    i = 1;
                }
                else
                {
                    c[i] = 0;
                    i++;
                }
            }
            return currBest;
        }
        
        //turn a route into a Hamiltonian Cycle with a path\distance weight.
        public hamilCycle calcCycleAndWeight(int[] Route, List<City> citList)
        {
            double x1, x2, y1, y2, weight = 0;
            
            //Cycle represents a complete path between all cities
            int[] Cycle = new int[Route.Length + 2];

            //prepend the node we ignored
            Cycle[0] = citList[0].CitNum;

            //populate middle of Cycle
            for (int i = 0; i < Route.Length; i++)
            {
                Cycle[i+1] = Route[i];
            }

            //append the node we ignored. Now we have a full cycle.
            //Route.Add(citList[0].CitNum);
            Cycle[Cycle.Length - 1] = Cycle[0];

            //iteratively calculate total weight of distance, node pair by node pair.
            for (int i = 0; i < Cycle.Length - 1; i++)
            {
                x1 = citList[Cycle[i]-1].CitX;
                x2 = citList[Cycle[i+1]-1].CitX;
                y1 = citList[Cycle[i]-1].CitY;
                y2 = citList[Cycle[i+1]-1].CitY;
                //distance formula
                weight += Math.Sqrt((x2 - x1)*(x2 - x1) + (y2 - y1)*(y2 - y1));
            }

            //return hamiltonian cycle to compare against current best cycle.
            hamilCycle hc = new hamilCycle(Cycle, weight);
            return hc;
        }
        
        //used in brute forcing TSP
        public void permute(string file)
        {
            List<City> citList = getCities(file);//get cities from .TSP file
            drawCities(citList);//draw the cities
            hamilCycle hc;//object to store final result

            //Now that we have the list of cities populated, we can start generating
            //all possible hamiltonian cycles and return the one with the shortest path.
            Stopwatch sw = Stopwatch.StartNew();
            hc = bruteForceTSP(citList.Count, citList);
            sw.Stop();
            TextBox tb3 = textBox3;
            tb3.Text = "Elapsed time: " + sw.ElapsedMilliseconds.ToString() + "ms";

            //output the final result to the UI
            string tmp = "Optimal Route to Travel: ";
            foreach (var x in hc.Cycle)
            {
                tmp += x.ToString() + ", ";
            }

            TextBox tb1 = textBox1;
            tb1.Text = tmp;
            TextBox tb2 = textBox2;
            tb2.Text = "Total distance: " + hc.PathWeight.ToString();
            //draw the solution
            drawCycle(citList, hc);
        }
        
        //This method is called from the UI to solve normal TSP's using brute force
        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "TSP files (*.tsp)|*.tsp";
            ofd.InitialDirectory = @"c:\";
            if (ofd.ShowDialog() == true)
            {
                string fn = ofd.FileName;
                permute(fn);
            }
        }
        #endregion

        #region PROJECT 2 STUFF
        //Hardcoded Edge List (for BFS and DFS)
        public List<List<int>> HEL()
        {
            List<List<int>> hel = new List<List<int>>();
            hel.Add(new List<int> { 1, 2, 3 });
            hel.Add(new List<int> { 2 });
            hel.Add(new List<int> { 3, 4 });
            hel.Add(new List<int> { 4, 5, 6 });
            hel.Add(new List<int> { 6, 7 });
            hel.Add(new List<int> { 7 });
            hel.Add(new List<int> { 8, 9 });
            hel.Add(new List<int> { 8, 9, 10 });
            hel.Add(new List<int> { 10 });
            hel.Add(new List<int> { 10 });
            return hel;
        }

        //draws the order of traversal for DFS and BFS traversal.
        public void drawTraversal(List<City> citList, String orderOfTraversal)
        {
            Canvas myCanvas = canv1;
            var cities = citList;

            //get order of traversal
            char[] delims = { ',' };
            String[] temp = orderOfTraversal.Split(delims);
            for (int x = 0; x < temp.Count() - 1; x++)
            {
                Line myPoint = new Line();
                myPoint.Stroke = System.Windows.Media.Brushes.Green;
                myPoint.X1 = cities[Int32.Parse(temp[x]) - 1].CitX * 2;
                myPoint.X2 = cities[Int32.Parse(temp[x + 1]) - 1].CitX * 2;
                myPoint.Y1 = cities[Int32.Parse(temp[x]) - 1].CitY * 2;
                myPoint.Y2 = cities[Int32.Parse(temp[x + 1]) - 1].CitY * 2;
                myPoint.HorizontalAlignment = HorizontalAlignment.Left;
                myPoint.VerticalAlignment = VerticalAlignment.Center;
                myPoint.StrokeThickness = 2;
                myCanvas.Children.Add(myPoint);
            }
        }

        //This method is called from the UI to solve TSP Digraphs using DFS
        private void btn_bfs_Click(object sender, RoutedEventArgs e)
        {
            List<City> citList = new List<City>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "TSP files (*.tsp)|*.tsp";
            ofd.InitialDirectory = @"c:\";
            if (ofd.ShowDialog() == true)
            {
                string fn = ofd.FileName;
                citList = bfs_solver(fn);

                drawCities(citList);//draw city nodes
                TextBox tb = txtb_diGraph;//get order from UI
                drawTraversal(citList, tb.Text);//draw path
            }
        }

        //This method is called from the UI to solve TSP Digraphs using BFS
        private void btn_dfs_Click(object sender, RoutedEventArgs e)
        {
            List<City> citList = new List<City>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "TSP files (*.tsp)|*.tsp";
            ofd.InitialDirectory = @"c:\";
            if (ofd.ShowDialog() == true)
            {
                string fn = ofd.FileName;
                citList = dfs_solver(fn);

                drawCities(citList);//draw city nodes
                TextBox tb = txtb_diGraph;//get order from UI
                drawTraversal(citList, tb.Text);//draw path
            }
        }

        //produces order of traversal for Breadth First Search
        public List<City> bfs_solver(string file)
        {
            String order = "";
            List<List<int>> hel = HEL();
            List<City> citList = getCities(file);
            int gn = citList[citList.Count - 1].CitNum;//obtain citNum of end city. This is our goal node.
            Queue<City> q = new Queue<City>();
            q.Enqueue(citList[0]);//enqueue first city (parent or starting city)
            while (q.Count > 0)
            {
                City current = q.Dequeue();//set current position (working node)
                //don't process goal node (assumed to be last city in citList)
                if (current == null || current.CitNum == gn)
                    continue;

                //need to enqueue adjacent cities from HEL
                foreach (int adjCit in hel[current.CitNum - 1])//for each adjacent or next city of current
                {
                    //be sure that duplicates are not added to queue 
                    //(no need to revisit cities already visited in a previous step)
                    if (!q.Contains(citList[adjCit]))
                    {
                        q.Enqueue(citList[adjCit]);
                    }
                }
                //Build the order of traversal.
                order += current.CitNum.ToString() + ",";
            }
            //add last city, the goal city, to final result.
            //output to UI
            order += citList[citList.Count - 1].CitNum.ToString();
            TextBox tb = txtb_diGraph;
            tb.Text = order;
            return citList;
        }

        //produces order of traversal for Depth-First Search
        public List<City> dfs_solver(string file)
        {
            String order = "";
            List<List<int>> hel = HEL();
            List<City> citList = getCities(file);
            List<City> visitedCities = new List<City>();
            Stack<City> s = new Stack<City>();
            int gn = citList[citList.Count - 1].CitNum;//obtain citNum of end city. This is our goal node.
            s.Push(citList[0]);//push first city (parent or starting city)
            while (s.Count > 0)
            {
                City current = s.Pop();//set current position (working node)
                if (current == null)
                    continue;
                if (!visitedCities.Contains(current))
                {
                    //need to push adjacent cities from HEL
                    visitedCities.Add(current);
                    //Build the order of traversal.
                    order += current.CitNum.ToString() + ",";
                    if (current.CitNum != gn)//no parsing to do on last node
                    {
                        //for each adjacent or next city of current
                        foreach (int adjCit in hel[current.CitNum - 1])
                        {
                            //be sure that duplicates are not added to stack 
                            //(no need to revisit cities already visited in a previous step)
                            if (!visitedCities.Contains(citList[adjCit]))
                            {
                                s.Push(citList[adjCit]);
                            }
                        }
                    }
                }
            }
            String finalorder = order.Substring(0, order.Length - 1);
            TextBox tb = txtb_diGraph;
            tb.Text = finalorder;
            return citList;
        }
        #endregion

        #region PROJECT 3 STUFF
        //Adapted from http://www.java2s.com/Code/CSharp/Development-Class/DistanceFromPointToLine.htm
        //Performs point to line distance using one city as the point, and two cities as the line
        //startLine will always be the last inserted city, endLine will be either its left or right neighbor
        public static double DistanceFromPointToLine(City point, City startLine, City endLine)
        {
            Double x = Math.Abs((endLine.CitX - startLine.CitX) * (startLine.CitX - point.CitY) 
                - (startLine.CitX - point.CitX) * (endLine.CitY - startLine.CitY))
                /Math.Sqrt(Math.Pow(endLine.CitX - startLine.CitX,2)+Math.Pow(endLine.CitY - startLine.CitY,2));
            return x;
        }

        private void btn_greed_Click(object sender, RoutedEventArgs e)
        {
            List<City> citList = new List<City>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "TSP files (*.tsp)|*.tsp";
            ofd.InitialDirectory = @"c:\";
            if (ofd.ShowDialog() == true)
            {
                string fn = ofd.FileName;
                citList = getCities(fn);

                Stopwatch sw = Stopwatch.StartNew();
                hamilCycle hc = greedyEdge(citList);
                sw.Stop();

                TextBox tb3 = textBox3;
                tb3.Text = "Elapsed time: " + sw.ElapsedMilliseconds.ToString() + "ms";
                drawCycle(citList, hc);//draw final solution
            }
        }

        public hamilCycle greedyEdge(List<City> citList)
        {
            hamilCycle hc;//basically finalRoute but with a total distance cost
            List<City> finalRoute = new List<City>();
            City lastAdded;//last inserted city in finalRoute
            List<City> unviscitList = new List<City>(citList);//list of unvisited cities

            //Find the left most city as our starting point. Do this to hopefully avoid crossed edges.
            double smallX = citList[0].CitX;//init smallX
            City LM = citList[0];//init left most city
            foreach (City c in unviscitList)
            {
                if (c.CitX < smallX)
                {
                    smallX = c.CitX;
                    LM = c;//set left most city
                }
            }
            finalRoute.Add(LM);//Add starting city to final route
            lastAdded = LM;//set last added city
            unviscitList.Remove(LM);//Remove LM city since we "visited it"
            //Now find the nearest point to LM and add it to final route.
            double nearx = 142;//init to this since this is assumed to be the greatest possible dist between any two cities
            foreach (City c in unviscitList)//for each unvisited city
            {
                //simple distance calculation between two points
                double tmp = Math.Sqrt((c.CitX - LM.CitX) * (c.CitX - LM.CitX) + (c.CitY - LM.CitY) * (c.CitY - LM.CitY));
                if (tmp < nearx) {
                    nearx = tmp;
                    lastAdded = c;
                }
            }
            finalRoute.Add(lastAdded);//add nearest neighbor
            unviscitList.Remove(lastAdded);//"visit" it
            //Now add the first city again so we have a full cycle (1,2,1)
            finalRoute.Add(finalRoute[0]);

            //At this stage, the 2nd city in our final route is our last added.
            //Now move into the meat of calculations.
            //for each remaining city that hasn't been visited
            int count = unviscitList.Count;
            for (int i = 0; i < count; i++)
            {
                //"Working Edges" defined by (lastAdded, finalRoute[index of lastAdded - 1]) 
                //and (lastAdded, finalRoute[index of lastAdded + 1])
                //index of lastAdded in finalRoute
                int index = finalRoute.FindIndex(City => City.CitNum.Equals(lastAdded.CitNum)) - 1;
                City endPoint = finalRoute[index];//City connected by left edge of lastAdded

                //for both working edges, compare the closest node. Add the closest of the two.
                City closestLE = unviscitList[0];//init closest city of Left Edge
                //need to find the distance of closest to line Left of lastAdded
                double distLE = DistanceFromPointToLine(closestLE, lastAdded, endPoint);
                for (int j = 1; j < unviscitList.Count; j++)
                {
                    double temp = DistanceFromPointToLine(unviscitList[j], lastAdded, endPoint);//line is the two working cities
                    if (temp < distLE)
                    {//if the city being evaluated is closer, set closet to it
                        distLE = temp;
                        closestLE = unviscitList[j];
                    }
                }

                index = finalRoute.FindIndex(City => City.CitNum.Equals(lastAdded.CitNum)) + 1;
                endPoint = finalRoute[index];//City connected by right edge of lastAdded
                City closestRE = unviscitList[0];//init closest city of Right Edge
                //need to find the distance of closest to line Right of lastAdded
                double distRE = DistanceFromPointToLine(closestRE, lastAdded, endPoint);
                for (int k = 1; k < unviscitList.Count; k++)
                {
                    double temp = DistanceFromPointToLine(unviscitList[k], lastAdded, endPoint);//line is the two working cities
                    if (temp < distRE)
                    {//if the city being evaluated is closer, set closet to it
                        distRE = temp;
                        closestRE = unviscitList[k];
                    }
                }

                //To determine the node closest to its working edge
                if (distRE < distLE)
                {
                    finalRoute.Insert(index, closestRE);//insert the city
                    lastAdded = closestRE;//update lastAdded
                    unviscitList.Remove(lastAdded);
                }
                else
                {
                    index = finalRoute.FindIndex(City => City.CitNum.Equals(lastAdded.CitNum));
                    finalRoute.Insert(index, closestLE);
                    lastAdded = closestLE;//update lastAdded
                    unviscitList.Remove(lastAdded);
                }
            }
            int[] route = new int[finalRoute.Count];
            for (int l = 0; l < finalRoute.Count; l++)
            {
                route[l] = finalRoute[l].CitNum;
            }
            hc = calcCycleAndWeight(route, citList);

            TextBox tb = textBox1;
            tb.Text = "Final Route: ";
            foreach(City c in finalRoute)
            {
                tb.Text += c.CitNum.ToString() + ", ";
            }
            tb = textBox2;
            tb.Text = "Path Weight: " + hc.PathWeight;

            return hc;
        }
        #endregion
    }
}
