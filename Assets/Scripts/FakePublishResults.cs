using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FakePublishResults : MonoBehaviour
{
    // Public fields
    [Header("Public fields")]
    [SerializeField] private GameObject publishBox;

    // Start is called before the first frame update
    void Start()
    {
        publishBox.transform.position = Camera.main.transform.position + new Vector3(0, 0, 0.5f);
    }

    // Publish results and go back to main menu
    public void PublishResults()
    {
        //using MySqlConnection connection = new MySqlConnection(ConnectionString.stringBuilder.ConnectionString);
        //connection.Open();

        //foreach (var op in FakeInteractionV1.operations)
        //{
        //    string sql =
        //    "INSERT INTO OperacoesResultados(cod_ordemDeManutencao, cod_operacaoResultado, instrucao,  resultado, resultadoLeitura, TipoLeitura, parametroLeitura, status)" +
        //    "VALUES (@OMCode, @OpCode, @Instruction, @Resultado, @ResultadoLeitura, @TipoLeitura, @ParametroLeitura, @Status)";

        //    using MySqlCommand command = new MySqlCommand(sql, connection);
        //    command.ExecuteNonQuery();
        //}

        SceneManager.LoadScene("MainMenu");
    }

    public void GoBackToOperations()
    {
        SceneManager.LoadScene("Operations");
    }
}
