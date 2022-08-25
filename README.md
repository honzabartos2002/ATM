Pro zprovoznění projektu na cizím počítači je potřeba:

1. (Pokud ještě nemáte) Mít ve Visual Studiu nainstalováno ".NET desktop development" a "Data storage and processing"
V horní liště Visual Studia je záložka "Tools" a hned jako další možnost je "Get Tools and Features". Tam stáhnete a nainstalujete ".NET desktop development" a "Data storage and processing"

2. Připojit se k lokální databázi
Stiskem ctrl + alt + s se vám zobrazí "Server Explorer" Stiknete tlačítko "Connect to Database" Do "Server name" zkopírujte: (localdb)\MSSQLLocalDB Klikněte na "Attach a database file" a pomocí tlačítka "browse" vyberte soubor s databází "AtmData.mdf" který naleznete ve složce projektu / Database Do "Logical name" zkopírujte: AtmData

Nyní projekt můžete spustit, ale pokud by přece jen byl nějaký problém tak se můžete podívat na všechny funkce zde: https://www.youtube.com/watch?v=-0YBnBURwfw
