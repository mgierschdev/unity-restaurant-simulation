
    public class PathNode{
        private PathNode parent;
        private int[] position;
        private int costToStart; // G cost
        private int costToEnd; // H cost
        private double EuclidianCost;
        private int value = 0; // value in the position, 0 free, 1 obstacle, 2 visited

        public PathNode(int[] position, int costToStart, int costToEnd, int value){
            this.position = position;
            this.costToStart = costToStart;
            this.costToEnd = costToEnd;
            this.value = value;
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

        public int GetValue(){
            return value;
        }

        public void SetParent(PathNode parent){
            this.parent = parent;
        }
        
        // F Cost = G + H
        public int GetTotalCost(){
            return costToStart + costToEnd;
        }
        
        public void SetValue(int value){
            this.value = value;
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