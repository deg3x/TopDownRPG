using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class DeveloperConsole : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Increasing the maximum output requires more memory")]
    private int maxOutputLines = 100;
    [SerializeField]
    private Text consoleOutputText;
    [SerializeField]
    private InputField consoleInputField;
    [SerializeField]
    private GameObject consoleObject;

    [Header("Colors")]

    [SerializeField]
    private Color defaultTextColor;
    [SerializeField]
    private Color warningTextColor;
    [SerializeField]
    private Color errorTextColor;

    public static DeveloperConsole instance { get; private set; }

    private Dictionary<string, CommandDescriptor> availableCommands;
    private int currentOutputLines;
    private string outputString;
    private bool shouldChangeConsoleState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<DeveloperConsole>();
            
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        shouldChangeConsoleState = false;
        availableCommands = new Dictionary<string, CommandDescriptor>();
        currentOutputLines = 0;
        consoleOutputText.text = "";
        outputString = "";

        AddCommand("exit", new CommandDescriptor(CloseConsole, 0, "Closes the console window", false));
        AddCommand("clear", new CommandDescriptor(ClearConsole, 0, "Clear the console output", false));
        AddCommand("help", new CommandDescriptor(PrintAvailableCommands, 0, "Print information about the available commands"));
    }

    private void Update()
    {
        CheckConsoleEnableInput();
        SyncConsoleOutput();
    }

    public void AddCommand(string commandName, CommandDescriptor descriptor)
    {
        if (availableCommands.ContainsKey(commandName))
        {
            Debug.Log("Command " + commandName + " already exists. Skipping...");
            return;
        }

        availableCommands.Add(commandName, descriptor);
    }

    public void RemoveCommand(string commandName)
    {
        if (!availableCommands.ContainsKey(commandName))
        {
            Debug.Log("Command " + commandName + " does not exist in the current context. Ignoring removal...");
            return;
        }

        availableCommands.Remove(commandName);
    }

    public void SubmitCommand()
    {
        if (!Input.GetButtonDown("Submit"))
        {
            return;
        }

        if (consoleInputField.text.Trim() == "")
        {
            FocusInputField();
            return;
        }

        ExecuteCommand();
    }

    private void ExecuteCommand()
    {
        string command = consoleInputField.text;
        consoleInputField.text = "";
        FocusInputField();

        if (!availableCommands.ContainsKey(command))
        {
            CommandNotFound(command);
            return;
        }

        if (availableCommands[command].pipeToOutput)
        {
            AppendConsoleOutput(command);
        }

        availableCommands[command].action?.Invoke();

        CheckOutputLimit();
    }

    private void SyncConsoleOutput()
    {
        consoleOutputText.text += outputString;
        outputString = "";
    }

    private void PrintAvailableCommands()
    {
        var keys = new List<string>(availableCommands.Keys);

        for (int i = keys.Count - 1; i >= 0; i--)
        {
            AppendConsoleOutput("<i>" + keys[i] + ":</i>\n\t\t" + availableCommands[keys[i]].description, "\t");
        }
    }

    private void ClearConsole()
    {
        consoleOutputText.text = "";
        currentOutputLines = 0;
    }

    private void CloseConsole()
    {
        consoleObject.SetActive(false);
    }

    private void CheckConsoleEnableInput()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.C))
        {
            shouldChangeConsoleState = true;
        }
        else
        {
            if (!shouldChangeConsoleState)
            {
                return;
            }

            bool shouldEnable = !consoleObject.activeInHierarchy;

            consoleObject.SetActive(shouldEnable);
            shouldChangeConsoleState = false;
            
            if (shouldEnable)
            {
                FocusInputField();
            }
        }
    }

    private void CommandNotFound(string command)
    {
        AppendConsoleOutput(command);
        AppendConsoleOutput("Command " + command + " not found. Use 'help' for a list of available commands.", "\t", ConsoleOutputType.Error);
        
        CheckOutputLimit();
    }

    private void AppendConsoleOutput(string output, string prependString = "", ConsoleOutputType outputType = ConsoleOutputType.Info)
    {
        string message = output;
        string newline = currentOutputLines == 0 ? "" : "\n";

        outputString += prependString;

        switch (outputType)
        {
            case ConsoleOutputType.Info:
                outputString += newline + prependString + ColorTagOpen(defaultTextColor) + message + ColorTagClose();
                break;
            case ConsoleOutputType.Warning:
                outputString += newline + prependString + ColorTagOpen(warningTextColor) + message + ColorTagClose();
                break;
            case ConsoleOutputType.Error:
                outputString += newline + prependString + ColorTagOpen(errorTextColor) + "[Error]: " + message + ColorTagClose();
                break;
            default:
                break;
        }

        currentOutputLines++;
    }

    private void CheckOutputLimit()
    {
        if (currentOutputLines > maxOutputLines)
        {
            DeleteOutputLines(currentOutputLines - maxOutputLines);
        }
    }

    private void FocusInputField()
    {
        EventSystem.current.SetSelectedGameObject(consoleInputField.gameObject);
        consoleInputField.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private string ColorTagOpen(Color color)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">";
    }

    private string ColorTagClose()
    {
        return "</color>";
    }

    private void DeleteOutputLines(int numberOfLines)
    {
        if (numberOfLines <= 0)
        {
            return;
        }

        string tempOutput = consoleOutputText.text;

        for (int i = 0; i < numberOfLines; i++)
        {
            int substringIndex = tempOutput.IndexOf('\n') + 1;
            tempOutput = tempOutput.Substring(substringIndex, tempOutput.Length - substringIndex);
        }

        consoleOutputText.text = tempOutput;
        currentOutputLines -= numberOfLines;
    }
}

public enum ConsoleOutputType
{
    Info,
    Warning,
    Error
}

public struct CommandDescriptor
{
    public Action action;
    public int numberOfArguments;
    public string description;
    public bool pipeToOutput;

    public CommandDescriptor(Action act, int args, string desc = "No available description", bool pipe = true)
    {
        action = act;
        numberOfArguments = args;
        description = desc;
        pipeToOutput = pipe;
    }
}
