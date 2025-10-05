namespace Communication
{
    public interface IGeneralPurposeInputOutput
    {
        /// <summary>
        /// Sets the general purpose IO level
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="Id">general purpose IO id </param>
        /// <param name="mask">general purpose IO mask</param>
        /// <param name="level">If 1 we Activate else we deactivate</param>
        void SetGPIOLevel(uint Id, uint mask, uint level);

        /// <summary>
        /// When the level is 0 we are using the Gpio as output
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mask"> Put the mask to one</param>
        /// <param name="level"> Put the level to 0</param>
        void SetGPIODirection(uint Id, uint mask, uint level);
    }
}