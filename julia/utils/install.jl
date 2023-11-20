#
#       Script Julia: install.jl
#
#       Importa as bibliotecas necessárias para a execução do projeto.
#
#       Por Gabriel Ferreira (http://lattes.cnpq.br/2296704854464665)
#
#       Execução: julia julia/utils/install.jl
#   
#   1.0.0 - 20/11/2023
#       ➤ Instalação da biblioteca JSON para carregar o arquivo de dados.
#       ➤ Instalação da biblioteca Wavelets para processar os dados utilizando
#   a transformada. Também é utilizada a biblioteca complementar ContinuousWavelets.
#       ➤ Instalação da biblioteca Plots para imprimir gráficos a fim de visualizar
#   os dados.
#       ➤ Instalação da biblioteca Statistics para lidar com análises estatísticas dos dados.
#
import Pkg;

Pkg.add("JSON");
Pkg.add("Wavelets");
Pkg.add("ContinuousWavelets");
Pkg.add("Statistics");