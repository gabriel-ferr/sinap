#
#       Script Julia: install.jl
#
#       Executa o processamento dos dados utilizando das
#   ferramentas disponibilizadas pelo Julia. 
#
#       Por Gabriel Ferreira (http://lattes.cnpq.br/2296704854464665)
#
#       Execução: julia julia/run.jl <arquivos de dados>
#
#       Configuração dos canais:
#       FP1, F4, F3, C4, FP2, O2, C3, O1
#
#   1.0.0 - 20/11/2023
#       ➤ Carrega os dados a partir de um arquivo .JSON unificado.
#       ➤ Utiliza uma transformada de Wavelet para reduzir os ruídos na onda
#   coletada, sendo aplicado por canal para cada intervalo de coleta equivalente
#   a uma tarefa.
#
using JSON;
using Wavelets;
using Plots;
using Statistics;

#   Carrega o parser do JSON.
json = JSON.parsefile(ARGS[1]);

#   Abre o for para as coletas, ou seja, cada index refere-se a uma coleta de dados.
for collect = 1:size(json)[1]
    #   Abre o for para as imagens exibidas no decorrer da coleta.
    for task = 1:size(json[collect])[1]

        qualitativeValue = json[collect][task]["Qualitative"];
        yearsOld = json[collect][task]["YearsOld"];
        time = json[collect][task]["Time"];

        #   Abre o for para os canais da tarefa.
        for channel = 5:size(json[collect][task]["Channels"])[1]

            raw = Float32.(json[collect][task]["Channels"][channel]);

            m = mean(raw);
            mVec = range(m, m; length = size(raw)[1])

            denoised = denoise(raw, TI=true);
            graph = plot(time, raw);
            plot!(time, mVec)
            scatter!(time, denoised, mc=:red, ms=2, ma=0.5)

            png(graph, "test.png")
            break
        end
        break
    end
    break
end