namespace LinearOptimizationService
{
    public struct NutrientData
    {
        public double Calories { get; init; }
        public double Protein { get; init; }
        public double Calcium { get; init; }
        public double Iron { get; init; }
        public double VitaminA { get; init; }
        public double VitaminB1 { get; init; }
        public double VitaminB2 { get; init; }
        public double Niacin { get; init; }
        public double VitaminC { get; init; }

        public NutrientData(
            double calories,
            double protein,
            double calcium,
            double iron,
            double vitaminA,
            double vitaminB1,
            double vitaminB2,
            double niacin,
            double vitaminC)
        {
            this.Calories = calories;
            this.Protein = protein;
            this.Calcium = calcium;
            this.Iron = iron;
            this.VitaminA = vitaminA;
            this.VitaminB1 = vitaminB1;
            this.VitaminB2 = vitaminB2;
            this.Niacin = niacin;
            this.VitaminC = vitaminC;
        }

        public double this[Nutrient s]
        {
            get
            {
                return s switch
                {
                    Nutrient.Calories => this.Calories,
                    Nutrient.Protein => this.Protein,
                    Nutrient.Calcium => this.Calcium,
                    Nutrient.Iron => this.Iron,
                    Nutrient.VitaminA => this.VitaminA,
                    Nutrient.VitaminB1 => this.VitaminB1,
                    Nutrient.VitaminB2 => this.VitaminB2,
                    Nutrient.Niacin => this.Niacin,
                    Nutrient.VitaminC => this.VitaminC,
                    _ => throw new ArgumentException("Unknown nutrient"),
                };
            }
        }
    }
}
