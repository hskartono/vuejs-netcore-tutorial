<template>
    <div>
        <b-form-group id="fieldset-is-received" v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <div v-if="isAllowEdit">
                <b-input-group>
                    <b-form-file
                        v-if="currentModel == null || isReupload"
                        v-bind="$attrs"
                        v-model="inputModel"
                        @change="setUploadFile"
                        placeholder="Choose a file or drop it here..."
                        drop-placeholder="Drop file here..."
                    ></b-form-file>
                    <b-form-input v-if="currentModel && !isReupload" size="sm" readonly="readonly" v-model="currentModel.originalFilename"></b-form-input>
                    <b-input-group-append v-if="currentModel && !isReupload">
                        <b-button variant="light" class="btn-download" size="sm" @click="downloadFile"><b-icon-download /></b-button>
                        <b-button variant="danger" class="btn-upload" size="sm" @click="reuploadFile"><b-icon-trash-fill /></b-button>
                    </b-input-group-append>
                </b-input-group>
                <b-form-invalid-feedback id="input-name-live-feedback">
                    {{ errorMessage }}
                </b-form-invalid-feedback>
            </div>
            <div v-else>
                <a href="javascript:void" @click="downloadFile">{{ currentModel != null ? currentModel.originalFilename : "" }}</a>
            </div>
        </b-form-group>
    </div>
</template>
<script>
import { saveAs } from 'file-saver';
export default {
    components: {},
    props: ["model", "currentModel", "errorMessage", "isEditable", "maxFileSize", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            inputModel: this.model,
            inputErrorMessage: '',
            isReupload:false,
        }
    }, 
    methods: {
        setUploadFile(event) {
            if (event.target.files.length > 0) {
                let file = event.target.files[0];
                if (this.maxFileSize != null && this.maxFileSize != undefined) {
                    let fileSize = (file.size / 1048576).toFixed();
                    if (fileSize > this.maxFileSize) {
                        alert("Max Allowed File Size " + this.maxFileSize + " MB");
                        this.inputModel = null;
                        return;
                    }
                }
                this.inputModel = file;
                this.$emit('update:model', this.inputModel);
                this.$emit('change');
            }
        },
        async downloadFile() {
            if (this.currentModel != null) {
                let url = process.env.VUE_APP_API_URL + 'Attachment/download/' + this.currentModel.id;
                let fileName = this.currentModel.originalFilename;
                this.$http.get(url, {responseType: 'blob'}).then(response => {
                    if (response != null) {
                        saveAs(response.data, fileName);
                    }
                }).catch(error => {
                    this.$store.dispatch('addErrorMessageGlobal', error);
                    throw error;
                })
                
            }
        },
        reuploadFile() {
            this.isReupload = true;
        }
    },
     computed : {
        isAllowEdit : {
            get: function() {
                if (this.isEditable != undefined) return this.isEditable;
                else return true;
            }
        }
    },
    watch: {
        currentModel(newValue, oldValue) {
            this.isReupload = false;
        },
    }
}
</script>
<style lang="scss" scoped>
/deep/ .modal-dialog {
    height: 70vh;
    width: 60vh;
}
.input-group-text {
    font-size: 0.7rem !important;
}

.btn-open-camera {
    padding: 1px 6px !important;
}

.barcode-text {
    font-size: 2rem;
    text-align: center;
}

</style>