
    public class PathNode{
        private int[] position;
        private int costToStart;
        private int costToEnd;
        private double EuclidianCost;

        public PathNode(int[] position, int costToStart, int costToEnd, double EuclidianCost){
            this.position = position;
            this.costToStart = costToStart;
            this.costToEnd = costToEnd;
            this.EuclidianCost = EuclidianCost;
        }

        public int GetCostToStart(){
            return costToStart;
        }

        public int GetCostEnd(){
            return costToEnd;
        }

        public double GetEuclidianCost(){
            return EuclidianCost;
        }

        public int[] GetPosition(){
            return position;
        }

        public void SetCostToStart(int costToStart){
            this.costToStart = costToStart;
        }

        public void SetCostEnd(int costToEnd){
            this.costToEnd = costToEnd;
        }

        public void SetEuclidianCost(double EuclidianCost){
            this.EuclidianCost = EuclidianCost;
        }

        public void SetPosition(int[] position){
            this.position = position;
        }
    }