namespace ReadableCode {
    public class MayFly {
        public void SomeMethod() {
            var someVariable = string.Empty;
            var otherVariableUsedMuchLater = 0;
            
            // 30 lines later
            
            if (someVariable.Equals("whatever")) {
                // do this
            }
            
            // 10 lines later
            
            if (otherVariableUsedMuchLater > 10) {
                // do this
            }
        }
    }
}