# git-merger

Uno strumento da riga di comando .NET 9 che porta l'ultimo commit da ogni branch di un repository Git sorgente a un repository di destinazione, mantenendo la stessa struttura di branch.

## Sommario
- [Prerequisiti](#prerequisiti)
- [Configurazione](#configurazione)
- [Limitazioni](#limitazioni)
- [Esempio di Caso d'Uso](#esempio-di-caso-duso)
- [Come Funziona](#come-funziona)
- [Licenza](#licenza)

---

## Prerequisiti

Prima di utilizzare git-merger, assicurati di avere installato quanto segue:

### Software Richiesto
- **.NET 9 SDK** (o versione successiva)
  - Download da: https://dotnet.microsoft.com/download/dotnet/9.0
  - Verifica installazione: `dotnet --version`

- **Git** (versione 2.0 o successiva)
  - Download da: https://git-scm.com/downloads
  - Verifica installazione: `git --version`

### Requisiti di Piattaforma
- **Windows**: Completamente supportato (utilizza Robocopy per le operazioni sui file)
- **Linux/macOS**: Richiede modifiche (Robocopy è specifico per Windows)

### Requisiti di Conoscenza
- Comprensione di base dei concetti di Git (branch, commit, repository)
- Familiarità con le interfacce da riga di comando

---

## Configurazione

### 1. Clonare o Scaricare il Repository

```bash
git clone https://github.com/phenixita/git-merger.git
cd git-merger
```

### 2. Compilare il Progetto

```bash
# Ripristinare le dipendenze e compilare
dotnet build GitMerger.sln --configuration Release

# Oppure compilare in modalità debug
dotnet build GitMerger.sln
```

### 3. Eseguire l'Applicazione

#### Opzione A: Utilizzando dotnet run
```bash
cd GitMerger
dotnet run
```

#### Opzione B: Utilizzando l'eseguibile compilato
```bash
# Dopo la compilazione in modalità Release
cd GitMerger/bin/Release/net9.0
dotnet GitMerger.dll

# Oppure su Windows
GitMerger.exe
```

### 4. Preparare i Tuoi Repository

Prima di eseguire lo strumento, hai bisogno di:

1. **Repository Sorgente**: Un repository Git esistente con i branch che vuoi portare
2. **Repository di Destinazione**: Un repository Git esistente (può essere vuoto o inizializzato con `git init`)

```bash
# Esempio: Creare un repository di destinazione
mkdir target-repo
cd target-repo
git init
cd ..
```

---

## Limitazioni

### Limitazioni di Piattaforma
- **Solo Windows**: Lo strumento utilizza `Robocopy` (un'utilità specifica per Windows) per le operazioni di copia dei file. Su Linux/macOS, sarebbe necessario implementare un `ICopyService` alternativo che utilizzi comandi di copia file appropriati per la piattaforma.

### Limitazioni Funzionali
1. **Solo Ultimo Commit**: Lo strumento porta solo l'**ultimo commit** da ogni branch, non l'intera cronologia dei commit.

2. **Struttura dei Branch**: 
   - Crea branch nel repository di destinazione che corrispondono ai nomi dei branch del repository sorgente
   - Rimuove il prefisso `origin/` dai nomi dei branch remoti
   - Utilizza "master" come branch radice predefinito (hardcoded)

3. **Configurazione Git**:
   - Utilizza una firma di test hardcoded per i commit:
     - Nome: "test"
     - Email: "test@mail.eu"
   - Dovresti modificare `Program.cs` per utilizzare informazioni appropriate sull'autore

4. **Operazioni sui File**:
   - Esclude la directory `.git` durante la copia (come previsto)
   - Mette in stage tutti i file (`*`) nel repository di destinazione
   - Cattura e registra silenziosamente le eccezioni di commit

5. **Console Interattiva**:
   - Richiede input manuale per i percorsi dei repository
   - Nessun supporto per argomenti da riga di comando
   - La validazione dell'input è basilare (controlla solo stringhe null/vuote)

6. **Nessuna Risoluzione dei Conflitti di Merge**:
   - Se si verificano conflitti durante il processo di porting, devono essere risolti manualmente
   - Lo strumento non fornisce meccanismi di risoluzione dei conflitti

### Limitazioni Tecniche
- Richiede che entrambi i repository siano repository Git validi
- Richiede accesso in scrittura al repository di destinazione
- Può fallire se i nomi dei branch contengono caratteri speciali
- Nessuna indicazione di progresso per repository di grandi dimensioni

---

## Esempio di Caso d'Uso

### Scenario
Hai un grande repository legacy con più branch di funzionalità. Vuoi creare un nuovo repository che contenga solo lo stato più recente di ogni branch (senza la cronologia completa) per:
- Ridurre la dimensione del repository
- Iniziare da zero con una cronologia pulita
- Migrare a una nuova struttura preservando l'organizzazione dei branch

### Esempio Passo-Passo

#### 1. Configurare i Repository

**Struttura del Repository Sorgente:**
```
legacy-project/
├── master (ultimo commit: "Update README")
├── feature/authentication (ultimo commit: "Add OAuth support")
└── feature/database (ultimo commit: "Optimize queries")
```

**Creare il Repository di Destinazione:**
```bash
# Creare e inizializzare il repository di destinazione
mkdir new-project
cd new-project
git init
cd ..
```

#### 2. Eseguire git-merger

```bash
# Navigare all'applicazione compilata
cd git-merger/GitMerger/bin/Release/net9.0

# Eseguire lo strumento
dotnet GitMerger.dll
```

#### 3. Fornire l'Input

Lo strumento richiederà tre input:

```
Cartella repo partenza:
C:\path\to\legacy-project

Cartella repo destinazione:
C:\path\to\new-project

Subdir:
src
```

**Spiegazione dell'Input:**
- **Cartella repo partenza** (Percorso del repository sorgente): Percorso completo al tuo repository Git sorgente
- **Cartella repo destinazione** (Percorso del repository di destinazione): Percorso completo al tuo repository Git di destinazione
- **Subdir**: Sottodirectory all'interno del repository di destinazione dove verranno posizionati i file (es. "src", "code", o "." per la radice)

#### 4. Risultato

**Struttura del Repository di Destinazione Dopo il Porting:**
```
new-project/
├── master
│   └── src/
│       └── [file dal branch master di legacy-project]
├── feature/authentication
│   └── src/
│       └── [file dal branch feature/authentication di legacy-project]
└── feature/database
    └── src/
        └── [file dal branch feature/database di legacy-project]
```

Ogni branch in `new-project` contiene:
- Un commit con messaggio: "Import [nome-branch]"
- Gli ultimi file dal branch corrispondente in `legacy-project`
- File posizionati nella sottodirectory specificata (`src` in questo esempio)

#### 5. Verificare i Risultati

```bash
cd new-project

# Controllare che i branch siano stati creati
git branch -a

# Esaminare un branch specifico
git checkout feature/authentication
git log --oneline
git ls-files
```

### Casi d'Uso Reali

1. **Suddivisione di Repository**: Estrarre branch specifici da un monorepo in un repository separato
2. **Cronologia Pulita**: Iniziare un nuovo repository con gli stati attuali dei branch, lasciando indietro la cronologia disordinata
3. **Migrazione di Archivi**: Creare snapshot degli stati dei branch per scopi di archiviazione
4. **Organizzazione del Codice**: Ristrutturare i file in sottodirectory durante la migrazione
5. **Consolidamento dei Branch**: Riunire branch da più repository sorgente

---

## Come Funziona

### Flusso del Processo

1. **Iterazione dei Branch**: Itera attraverso tutti i branch nel repository sorgente
2. **Checkout**: Effettua il checkout di ogni branch nel repository sorgente
3. **Creazione del Branch**: Crea i branch corrispondenti nel repository di destinazione (se non esistono)
4. **Copia dei File**: Utilizza Robocopy per copiare i file (escludendo `.git`) nella posizione di destinazione
5. **Commit**: Mette in stage tutte le modifiche e crea un commit nel repository di destinazione
6. **Ripeti**: Processa tutti i branch in sequenza

### Componenti Principali

- **GitMerger.Core**: Libreria contenente la logica principale
  - `GitMerger.CloneLastCommitOfAllBranches()`: Metodo di orchestrazione principale
  - `GitMerger.CloneBranch()`: Gestisce il porting di singoli branch
  - `ICopyService`: Interfaccia per le operazioni di copia dei file
  - `RobocopyService`: Implementazione della copia dei file specifica per Windows

- **GitMerger**: Applicazione console che fornisce l'interfaccia utente

### Stack Tecnologico

- **.NET 9**: Framework moderno multipiattaforma
- **LibGit2Sharp 0.30.0**: Binding .NET per libgit2 (operazioni Git)
- **Robocopy**: Utilità Windows per la copia dei file

---

## Licenza

Questo progetto è rilasciato sotto licenza MIT - vedere il file [LICENSE](LICENSE) per i dettagli.

Copyright (c) 2017 phenixita
